using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Winter2022_PassionProject_N01519420.Models;
using Winter2022_PassionProject_N01519420.Models.ViewModels;
using System.Diagnostics;
using System.Web;
using System.IO;

namespace Winter2022_PassionProject_N01519420.Controllers
{
    public class MovieDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Display List of all movies
        /// </summary>
        /// <returns>list of movies</returns>
        // GET: api/MovieData/ListMovies
        [HttpGet]
        public IEnumerable<MovieDto> ListMovies()
        {
            List<Movie> Movies = db.movies.OrderBy(m => m.MovieName).ToList();
            List<MovieDto> MovieDtos = new List<MovieDto>();
            Movies.ForEach(m => MovieDtos.Add(new MovieDto()
            {
                MovieID = m.MovieID,
                MovieName = m.MovieName,
                ReleaseYear = m.ReleaseYear
            }));
            return MovieDtos;
        }

        /// <summary>
        /// display list of movies for perticular actor
        /// </summary>
        /// <param name="id">passing parameter actor id</param>
        /// <returns>list of movies for perticular actor</returns>
        // GET: api/MovieData/ListMoviesForActor/1
        [HttpGet]
        [ResponseType(typeof(MovieDto))]
        public IHttpActionResult ListMoviesForActor(int id)
        {
            List<Movie> Movies = db.movies.Where(
                m => m.actors.Any(
                    a => a.ActorID == id
            )).ToList();
            List<MovieDto> MovieDtos = new List<MovieDto>();

            Movies.ForEach(m => MovieDtos.Add(new MovieDto()
            {
                MovieID = m.MovieID,
                MovieName = m.MovieName,
                ReleaseYear = m.ReleaseYear
            }));
            return Ok(MovieDtos);
        }

        /// <summary>
        /// display record for perticular movie
        /// </summary>
        /// <param name="id">passing parameter movie id</param>
        /// <returns>details of perticular movie</returns>
        // GET: api/MovieData/FindMovie/5
        [HttpGet]
        [ResponseType(typeof(Movie))]
        public IHttpActionResult FindMovie(int id)
        {
            Movie Movie = db.movies.Find(id);
            MovieDto MovieDto = new MovieDto()
            {
                MovieID = Movie.MovieID,
                MovieName = Movie.MovieName,
                MovieHasPic = Movie.MovieHasPic,
                PicExtension = Movie.PicExtension,
                ReleaseYear = Movie.ReleaseYear
            };
            if (Movie == null)
            {
                return NotFound();
            }

            return Ok(MovieDto);
        }

        /// <summary>
        /// add new actor in the perticular movie
        /// </summary>
        /// <param name="movieid">passing parameter movie id</param>
        /// <param name="actorid">passing parameter actor id</param>
        /// <returns>add new actor in the perticular movie</returns>
        // POST: api/MovieData/AssociateMovieWithActor/2/7
        [ResponseType(typeof(void))]
        [HttpPost]
        [Route("api/MovieData/AssociateMovieWithActor/{movieid}/{actorid}")]
        public IHttpActionResult AssociateMovieWithActor(int movieid, int actorid)
        {
            Movie SelectedMovie = db.movies.Include(m => m.actors).Where(m => m.MovieID == movieid).FirstOrDefault();
            Actor SelectedActor = db.actors.Find(actorid);

            SelectedMovie.actors.Add(SelectedActor);
            db.SaveChanges();
            return Ok();
        }

        /// <summary>
        /// remove actor from perticular movie
        /// </summary>
        /// <param name="movieid">passing parameter movie id</param>
        /// <param name="actorid">passing parameter actor id</param>
        /// <returns>remove actor from the perticular movie</returns>
        // POST: api/MovieData/UnAssociateMovieWithActor/2/7
        [ResponseType(typeof(void))]
        [HttpPost]
        [Route("api/MovieData/UnAssociateMovieWithActor/{movieid}/{actorid}")]
        public IHttpActionResult UnAssociateMovieWithActor(int movieid, int actorid)
        {
            Movie SelectedMovie = db.movies.Include(m => m.actors).Where(m => m.MovieID == movieid).FirstOrDefault();
            Actor SelectedActor = db.actors.Find(actorid);

            SelectedMovie.actors.Remove(SelectedActor);
            db.SaveChanges();
            return Ok();
        }

        /// <summary>
        /// update perticular movie information
        /// </summary>
        /// <param name="id">passing parameter movie id</param>
        /// <param name="movie">movie entity</param>
        /// <returns>updated movie record</returns>
        // POST: api/MovieData/UpdateMovie/5
        [ResponseType(typeof(void))]
        [HttpPost]
        [Route("api/MovieData/UpdateMovie/{id}")]
        [Authorize]
        public IHttpActionResult UpdateMovie(int id, Movie movie)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != movie.MovieID)
            {
                return BadRequest();
            }
           
            db.Entry(movie).State = EntityState.Modified;
            db.Entry(movie).Property(m => m.MovieHasPic).IsModified = false;
            db.Entry(movie).Property(m => m.PicExtension).IsModified = false;
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovieExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Receives movie picture data, uploads it to the webserver and updates the movie's HasPic option
        /// </summary>
        /// <param name="id">the movie id</param>
        /// <returns>status code 200 if successful.</returns>
        /// <example>
        /// POST: api/movieData/UpdatemoviePic/3
        /// HEADER: enctype=multipart/form-data
        /// FORM-DATA: image
        /// </example>
        /// https://stackoverflow.com/questions/28369529/how-to-set-up-a-web-api-controller-for-multipart-form-data

        [HttpPost]
        public IHttpActionResult UploadMoviePic(int id)
        {

            bool haspic = false;
            string picextension;
            if (Request.Content.IsMimeMultipartContent())
            {
                Debug.WriteLine("Received multipart form data.");

                int numfiles = HttpContext.Current.Request.Files.Count;
                Debug.WriteLine("Files Received: " + numfiles);

                //Check if a file is posted
                if (numfiles == 1 && HttpContext.Current.Request.Files[0] != null)
                {
                    var moviePic = HttpContext.Current.Request.Files[0];
                    //Check if the file is empty
                    if (moviePic.ContentLength > 0)
                    {
                        //establish valid file types (can be changed to other file extensions if desired!)
                        var valtypes = new[] { "jpeg", "jpg", "png", "gif" };
                        var extension = Path.GetExtension(moviePic.FileName).Substring(1);
                        //Check the extension of the file
                        if (valtypes.Contains(extension))
                        {
                            try
                            {
                                //file name is the id of the image
                                string fn = id + "." + extension;

                                //get a direct file path to ~/Content/movies/{id}.{extension}
                                string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/Images/Movies/"), fn);

                                //save the file
                                moviePic.SaveAs(path);

                                //if these are all successful then we can set these fields
                                haspic = true;
                                picextension = extension;

                                //Update the movie haspic and picextension fields in the database
                                Movie Selectedmovie = db.movies.Find(id);
                                Selectedmovie.MovieHasPic = haspic;
                                Selectedmovie.PicExtension = extension;
                                db.Entry(Selectedmovie).State = EntityState.Modified;

                                db.SaveChanges();

                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("movie Image was not saved successfully.");
                                Debug.WriteLine("Exception:" + ex);
                                return BadRequest();
                            }
                        }
                    }

                }

                return Ok();
            }
            else
            {
                //not multipart form data
                return BadRequest();

            }

        }

        /// <summary>
        /// add new movie information into database
        /// </summary>
        /// <param name="movie">movie entity</param>
        /// <returns>added new movie record</returns>
        // POST: api/MovieData/AddMovie
        [ResponseType(typeof(Movie))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult AddMovie(Movie movie)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.movies.Add(movie);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = movie.MovieID }, movie);
        }

        /// <summary>
        /// delete perticular movie from the database
        /// </summary>
        /// <param name="id">passing parameter movie id</param>
        /// <returns>remove perticular movie record</returns>
        // POST: api/MovieData/DeleteMovie/5
        [ResponseType(typeof(Movie))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult DeleteMovie(int id)
        {
            Movie movie = db.movies.Find(id);
            if (movie == null)
            {
                return NotFound();
            }

            db.movies.Remove(movie);
            db.SaveChanges();

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MovieExists(int id)
        {
            return db.movies.Count(e => e.MovieID == id) > 0;
        }
    }
}