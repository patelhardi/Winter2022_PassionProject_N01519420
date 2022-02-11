using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Http;
using Winter2022_PassionProject_N01519420.Models;
using Winter2022_PassionProject_N01519420.Models.ViewModels;
using System.Diagnostics;
using System.Web.Script.Serialization;

namespace Winter2022_PassionProject_N01519420.Controllers
{
    public class ActorController : Controller
    {
        // Can use in the code later 
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static ActorController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44313/api/");
        }

        /// <summary>
        /// Display list of all actors
        /// Select * from actors
        /// </summary>
        /// <returns>list of all actors</returns>
        // GET: Actor/ListAll
        public ActionResult ListAll()
        {
            //communicate with datacontroller class
            string url = "actordata/listactors";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<ActorDto> actors = response.Content.ReadAsAsync<IEnumerable<ActorDto>>().Result;
            
            return View(actors);
        }

        /// <summary>
        /// Display list of actors as per search box value
        /// </summary>
        /// <param name="SearchKey">search box value</param>
        /// <returns>list of actors</returns>
        /// <example>Actor/List/Tom => Tom Cruise
        ///                            Tom Holland                           
        ///                            Tom Felton
        /// </example>
        // GET: Actor/List
        public ActionResult List(string SearchKey = null)
        {
            try
            {
                //communicate with datacontroller class
                string url = "actordata/listactorsbysearchkey/" + SearchKey;
                HttpResponseMessage response = client.GetAsync(url).Result;
                IEnumerable<ActorDto> actors = response.Content.ReadAsAsync<IEnumerable<ActorDto>>().Result;
                
                return View(actors);
            }
            catch(AggregateException ae)
            {
                Debug.WriteLine(ae.Message);
                return RedirectToAction("Error");
            }
            
        }

        /// <summary>
        /// Select * from actor where id = 5
        /// </summary>
        /// <param name="id">passing parameter actor id</param>
        /// <returns>details of perticular actor</returns>
        /// <example>Actor/Details/5 => Actor First Name: Millie
        ///                             Actor Last Name: Bobby Brown</example>
        // GET: Actor/Details/5
        public ActionResult Details(int id)
        {
            DetailsActor viewModel = new DetailsActor();

            //communicate with datacontroller class
            string url = "actordata/findactor/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            ActorDto SelectedActor = response.Content.ReadAsAsync<ActorDto>().Result;

            viewModel.SelectedActor = SelectedActor;

            url = "moviedata/listmoviesforactor/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<MovieDto> KeptMovies = response.Content.ReadAsAsync<IEnumerable<MovieDto>>().Result;

            viewModel.KeptMovies = KeptMovies;

            return View(viewModel);
        }

        /// <summary>
        /// Display Error Page
        /// </summary>
        /// <returns>Error Page</returns>
        // GET: Actor/Error
        public ActionResult Error()
        {
            return View();
        }

        /// <summary>
        /// Display Add New actor page
        /// </summary>
        /// <returns>New Actor Page</returns>
        // GET: Actor/New
        public ActionResult New()
        {
            return View();
        }

        /// <summary>
        /// Insert new data into actor table
        /// Insert into actor (fname, lname) values ("aaa", "bbb")
        /// <param name="actor">Actor entity</param>
        /// <returns>Add Actor into the database</returns>
        // POST: Actor/Create
        [HttpPost]
        public ActionResult Create(Actor actor)
        {
            //create new object using json
            //communicate with data controller class
            string url = "actordata/addactor";
            string jsonpayload = jss.Serialize(actor);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListAll");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// Display Update Page
        /// </summary>
        /// <param name="id">passing perticular Actor id </param>
        /// <returns>Edit Actor page</returns>
        // GET: Actor/Edit/5
        public ActionResult Edit(int id)
        {
            UpdateActor viewModel = new UpdateActor();

            //communicate with data controller class
            string url = "actordata/findactor/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            ActorDto actor = response.Content.ReadAsAsync<ActorDto>().Result;

            viewModel.SelectedActor = actor;

            url = "moviedata/listmoviesforactor/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<MovieDto> MovieOptions = response.Content.ReadAsAsync<IEnumerable<MovieDto>>().Result;

            viewModel.MovieOptions = MovieOptions;

            return View(viewModel);
        }

        /// <summary>
        /// Upadte perticular actor
        /// update actor(fname, lname) set(newFname, newLname) where id=2
        /// </summary>
        /// <param name="id">perticular actor id</param>
        /// <param name="actor">Actor entity</param>
        /// <returns>upadate actor information into database</returns>
        // POST: Actor/Update/5
        [HttpPost]
        public ActionResult Update(int id, Actor actor)
        {
            //update object using json
            //communicate with data controller class
            string url = "actordata/updateactor/" + id;
            string jsonpayload = jss.Serialize(actor);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListAll");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        /// <summary>
        /// Display Delete page
        /// </summary>
        /// <param name="id">perticular actor id</param>
        /// <returns>delete page</returns>
        // GET: Actor/Delete/5
        public ActionResult DeleteConfirm(int id)
        {
            //communicate with data controller class
            string url = "actordata/findactor/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            ActorDto actor = response.Content.ReadAsAsync<ActorDto>().Result;
            
            return View(actor);
        }

        /// <summary>
        /// delete actor record for perticular actor
        /// delete actor where actorid = 5
        /// </summary>
        /// <param name="id">passing perticular actor id</param>
        /// <returns>delete record from database</returns>
        // POST: Actor/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            //communicate with data controller class
            string url = "actordata/deleteactor/" + id;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListAll");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
    }
}
