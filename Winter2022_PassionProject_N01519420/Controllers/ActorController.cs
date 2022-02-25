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
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                //cookies are manually set in RequestHeader
                UseCookies = false
            };

            client = new HttpClient(handler);
            client.BaseAddress = new Uri("https://localhost:44313/api/");
        }

        /// <summary>
        /// Grabs the authentication cookie sent to this controller.
        /// For proper WebAPI authentication, you can send a post request with login credentials to the WebAPI and log the access token from the response. The controller already knows this token, so we're just passing it up the chain.
        /// 
        /// Here is a descriptive article which walks through the process of setting up authorization/authentication directly.
        /// https://docs.microsoft.com/en-us/aspnet/web-api/overview/security/individual-accounts-in-web-api
        /// </summary>
        private void GetApplicationCookie()
        {
            string token = "";
            //HTTP client is set up to be reused, otherwise it will exhaust server resources.
            //This is a bit dangerous because a previously authenticated cookie could be cached for
            //a follow-up request from someone else. Reset cookies in HTTP client before grabbing a new one.
            client.DefaultRequestHeaders.Remove("Cookie");
            if (!User.Identity.IsAuthenticated) return;

            HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies.Get(".AspNet.ApplicationCookie");
            if (cookie != null) token = cookie.Value;

            //collect token as it is submitted to the controller
            //use it to pass along to the WebAPI.
            Debug.WriteLine("Token Submitted is : " + token);
            if (token != "") client.DefaultRequestHeaders.Add("Cookie", ".AspNet.ApplicationCookie=" + token);

            return;
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
        [Authorize]
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
        [Authorize]
        public ActionResult Create(Actor actor)
        {
            GetApplicationCookie();//get token credentials
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
        [Authorize]
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
        [Authorize]
        public ActionResult Update(int id, Actor actor, HttpPostedFileBase ActorPic)
        {
            GetApplicationCookie();//get token credentials
            //update object using json
            //communicate with data controller class
            string url = "actordata/updateactor/" + id;
            string jsonpayload = jss.Serialize(actor);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode && ActorPic != null)
            {
                url = "ActorData/UploadActorPic/" + id;

                MultipartFormDataContent requestcontent = new MultipartFormDataContent();
                HttpContent imagecontent = new StreamContent(ActorPic.InputStream);
                requestcontent.Add(imagecontent, "ActorPic", ActorPic.FileName);
                response = client.PostAsync(url, requestcontent).Result;
                return RedirectToAction("List");
            }
            else if (response.IsSuccessStatusCode)
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
        [Authorize]
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
        [Authorize]
        public ActionResult Delete(int id)
        {
            GetApplicationCookie();//get token credentials
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
