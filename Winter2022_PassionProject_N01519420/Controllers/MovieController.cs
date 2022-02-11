using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Winter2022_PassionProject_N01519420.Models;
using Winter2022_PassionProject_N01519420.Models.ViewModels;
using System.Net.Http;
using System.Web.Script.Serialization;
using System.Diagnostics;

namespace Winter2022_PassionProject_N01519420.Controllers
{
    public class MovieController : Controller
    {
        // Can use in the code later 
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static MovieController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44313/api/");
        }

        /// <summary>
        /// display list of all movies
        /// select * from movie
        /// </summary>
        /// <returns>list of movies</returns>
        // GET: Movie/List
        public ActionResult List()
        {
            //communicate wth data controller class
            string url = "moviedata/listmovies";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<MovieDto> movies = response.Content.ReadAsAsync<IEnumerable<MovieDto>>().Result;
            
            return View(movies);
        }

        /// <summary>
        /// display detail of perticular movie
        /// select * from movie where id = 5
        /// </summary>
        /// <param name="id">passing parameter movie id</param>
        /// <returns>details of perticular movie</returns>
        // GET: Movie/Details/5
        public ActionResult Details(int id)
        {
            DetailsMovie viewModel = new DetailsMovie();
            
            //communicate wth data controller class
            string url = "moviedata/findmovie/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            MovieDto SelectedMovie = response.Content.ReadAsAsync<MovieDto>().Result;

            viewModel.SelectedMovie = SelectedMovie;

            url = "actordata/listactorsformovie/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<ActorDto> KeptActors = response.Content.ReadAsAsync<IEnumerable<ActorDto>>().Result;

            viewModel.KeptActors = KeptActors;

            url = "actordata/listactorsnotinperticularmovie/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<ActorDto> ActorsOptions = response.Content.ReadAsAsync<IEnumerable<ActorDto>>().Result;

            viewModel.ActorsOptions = ActorsOptions;

            return View(viewModel);
        }

        /// <summary>
        /// add new actor in the perticular movie
        /// </summary>
        /// <param name="id">passing parameter movie id</param>
        /// <param name="ActorID">passing parameter actor id</param>
        /// <returns>add actor in the perticular movie</returns>
        //POST: Movie/Associate/{movieid}
        [HttpPost]
        public ActionResult Associate(int id, int ActorID)
        {
            //Debug.WriteLine("movie:" + MovieID + "actor:" + ActorID);
            //communicate with data controller class
            string url = "moviedata/associatemoviewithactor/" + id + "/" + ActorID;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            
            return RedirectToAction("Details/" + id);
        }

        /// <summary>
        /// remove actor from the perticular movie
        /// </summary>
        /// <param name="id">passing parameter movie id</param>
        /// <param name="ActorID">passing parameter actor id</param>
        /// <returns>remove actor from perticular movie</returns>
        //GET: Movie/UnAssociate/{id}?ActorID={ActorID}
        [HttpGet]
        public ActionResult UnAssociate(int id, int ActorID)
        {
            //communicate with data controller class
            string url = "moviedata/unassociatemoviewithactor/" + id + "/" + ActorID;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            
            return RedirectToAction("Details/" + id);
        }

        /// <summary>
        /// display error page
        /// </summary>
        /// <returns>error page</returns>
        // GET: Movie/Error
        public ActionResult Error()
        {
            return View();
        }

        /// <summary>
        /// display add new movie page
        /// </summary>
        /// <returns>add movie page</returns>
        // GET: Movie/New
        public ActionResult New()
        {
            return View();
        }

        /// <summary>
        /// add new movie into the database
        /// insert into movie(name, releaseyear) values(mName, mReleaseYear)
        /// </summary>
        /// <param name="movie">movie entity</param>
        /// <returns>list of movies</returns>
        // POST: Movie/Create
        [HttpPost]
        public ActionResult Create(Movie movie)
        {
            //create new object using json
            //communicate with data controller class
            string url = "moviedata/addmovie";
            string jsonpayload = jss.Serialize(movie);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// display edit page for perticular movie
        /// </summary>
        /// <param name="id">passing parameter movie id</param>
        /// <returns>edit page</returns>
        // GET: Movie/Edit/5
        public ActionResult Edit(int id)
        {
            UpdateMovie viewModel = new UpdateMovie();

            //communicate with data controller class
            string url = "moviedata/findmovie/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            MovieDto movie = response.Content.ReadAsAsync<MovieDto>().Result;
            
            viewModel.SelectedMovie = movie;

            url = "actordata/listactorsformovie/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<ActorDto> KeptActors = response.Content.ReadAsAsync<IEnumerable<ActorDto>>().Result;

            viewModel.KeptActors = KeptActors;

            return View(viewModel);
        }

        /// <summary>
        /// update perticular movie details into the database
        /// update movie set(name, release year) where id = 5
        /// </summary>
        /// <param name="id">passing parameter movie id</param>
        /// <param name="movie">movie entity</param>
        /// <returns>update movie information into database</returns>
        // POST: Movie/Update/5
        [HttpPost]
        public ActionResult Update(int id, Movie movie)
        {
            //create new object using json
            //communicate with data controller class
            string url = "moviedata/updatemovie/" + id;
            string jsonpayload = jss.Serialize(movie);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// display delete page for perticular movie id
        /// </summary>
        /// <param name="id">passing parameter movie id</param>
        /// <returns>delete page</returns>
        // GET: Movie/Delete/5
        public ActionResult DeleteConfirm(int id)
        {
            //communication with data controller class
            string url = "moviedata/findmovie/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            MovieDto movie = response.Content.ReadAsAsync<MovieDto>().Result;
            
            return View(movie);
        }

        /// <summary>
        /// delete perticular movie from the database
        /// </summary>
        /// <param name="id">passing parameter movie id</param>
        /// <returns>delete movie record from the database</returns>
        // POST: Movie/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            //communicate with data controller class
            string url = "moviedata/deletemovie/" + id;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
    }
}
