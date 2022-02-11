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
        /// add new movie information into database
        /// </summary>
        /// <param name="movie">movie entity</param>
        /// <returns>added new movie record</returns>
        // POST: api/MovieData/AddMovie
        [ResponseType(typeof(Movie))]
        [HttpPost]
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