using Microsoft.Ajax.Utilities;
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
using System.Diagnostics;
using System.Web;
using System.IO;

namespace Winter2022_PassionProject_N01519420.Controllers
{
    public class ActorDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Display list of all actors
        /// </summary>
        /// <returns>list of actors</returns>
        // GET: api/ActorData/ListActors
        [HttpGet]
        [Route("api/ActorData/ListActors")]
        public IEnumerable<ActorDto> ListActors()
        {
            List<Actor> Actors = db.actors.OrderBy(a => a.ActorFirstName).ToList();
            List<ActorDto> ActorDtos = new List<ActorDto>();
            Actors.ForEach(a => ActorDtos.Add(new ActorDto()
            {
                ActorID = a.ActorID,
                ActorFirstName = a.ActorFirstName,
                ActorLastName = a.ActorLastName,
                Bio = a.Bio,
                ActorHasPic = a.ActorHasPic,
                PicExtension = a.PicExtension
            }));
            return ActorDtos;
        }

        /// <summary>
        /// Display list of all actors as per search box value
        /// </summary>
        /// <param name="SearchKey">search box value</param>
        /// <returns>list of actors</returns>
        // GET: api/ActorData/ListActorsBySearchKey/Tom
        [HttpGet]
        [Route("api/ActorData/ListActorsBySearchKey/{SearchKey?}")]
        public IEnumerable<ActorDto> ListActorsBySearchKey(string SearchKey = null)
        {
            //Debug.WriteLine("Search key is:" + SearchKey);
            List<Actor> Actors = db.actors.Where(
                a => a.ActorFirstName.Contains(SearchKey)).ToList();
            List<ActorDto> ActorDtos = new List<ActorDto>();
            Actors.ForEach(a => ActorDtos.Add(new ActorDto()
            {
                ActorID = a.ActorID,
                ActorFirstName = a.ActorFirstName,
                ActorLastName = a.ActorLastName
            }));
            return ActorDtos;
        }

        /// <summary>
        /// display list of actors in perticular movie
        /// </summary>
        /// <param name="id">passing parameter movie id</param>
        /// <returns>list of actors</returns>
        // GET: api/ActorData/ListActorsForMovie/1
        [HttpGet]
        [ResponseType(typeof(ActorDto))]
        public IHttpActionResult ListActorsForMovie(int id)
        {
            List<Actor> Actors = db.actors.Where(
                a => a.movies.Any(
                    m => m.MovieID == id
            )).ToList();
            List<ActorDto> ActorDtos = new List<ActorDto>();

            Actors.ForEach(a => ActorDtos.Add(new ActorDto()
            {
                ActorID = a.ActorID,
                ActorFirstName = a.ActorFirstName,
                ActorLastName = a.ActorLastName
            }));
            return Ok(ActorDtos);
        }

        /// <summary>
        /// display list of actors that are not in perticular movie
        /// </summary>
        /// <param name="id">passing parameter movie id</param>
        /// <returns>list of actors</returns>
        // GET: api/ActorData/ListActorsNotInPerticularMovie/1
        [HttpGet]
        public IHttpActionResult ListActorsNotInPerticularMovie(int id)
        {
            List<Actor> Actors = db.actors.Where(
                a => !a.movies.Any(
                    m => m.MovieID == id
            )).OrderBy(a => a.ActorFirstName).ToList();
            List<ActorDto> ActorDtos = new List<ActorDto>();
            Actors.ForEach(a => ActorDtos.Add(new ActorDto()
            {
                ActorID = a.ActorID,
                ActorFirstName = a.ActorFirstName,
                ActorLastName = a.ActorLastName
            }));
            return Ok(ActorDtos); ;
        }

        /// <summary>
        /// Display details of perticular actor 
        /// </summary>
        /// <param name="id">passing parameter actor id</param>
        /// <returns>details of perticular actor</returns>
        // GET: api/ActorData/FindActor/5
        [ResponseType(typeof(Actor))]
        [HttpGet]
        public IHttpActionResult FindActor(int id)
        {
            Actor Actor = db.actors.Find(id);
            ActorDto ActorDto = new ActorDto()
            {
                ActorID = Actor.ActorID,
                ActorFirstName = Actor.ActorFirstName,
                ActorLastName = Actor.ActorLastName,
                Bio = Actor.Bio,
                ActorHasPic = Actor.ActorHasPic,
                PicExtension = Actor.PicExtension
            };
            if (Actor == null)
            {
                return NotFound();
            }

            return Ok(ActorDto);
        }

        /// <summary>
        /// Update perticular actor information
        /// </summary>
        /// <param name="id">passing parameter actor id</param>
        /// <param name="actor">actor entity</param>
        /// <returns>update actor information into database</returns>
        // POST: api/ActorData/UpdateActor/5
        [ResponseType(typeof(void))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult UpdateActor(int id, Actor actor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != actor.ActorID)
            {
                return BadRequest();
            }

            db.Entry(actor).State = EntityState.Modified;
            db.Entry(actor).Property(a => a.ActorHasPic).IsModified = false;
            db.Entry(actor).Property(a => a.PicExtension).IsModified = false;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActorExists(id))
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
        /// Receives actor picture data, uploads it to the webserver and updates the actor's HasPic option
        /// </summary>
        /// <param name="id">the actor id</param>
        /// <returns>status code 200 if successful.</returns>
        /// <example>
        /// POST: api/actorData/UpdateactorPic/3
        /// HEADER: enctype=multipart/form-data
        /// FORM-DATA: image
        /// </example>
        /// https://stackoverflow.com/questions/28369529/how-to-set-up-a-web-api-controller-for-multipart-form-data

        [HttpPost]
        public IHttpActionResult UploadActorPic(int id)
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
                    var actorPic = HttpContext.Current.Request.Files[0];
                    //Check if the file is empty
                    if (actorPic.ContentLength > 0)
                    {
                        //establish valid file types (can be changed to other file extensions if desired!)
                        var valtypes = new[] { "jpeg", "jpg", "png", "gif" };
                        var extension = Path.GetExtension(actorPic.FileName).Substring(1);
                        //Check the extension of the file
                        if (valtypes.Contains(extension))
                        {
                            try
                            {
                                //file name is the id of the image
                                string fn = id + "." + extension;

                                //get a direct file path to ~/Content/actors/{id}.{extension}
                                string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Content/Images/Actors/"), fn);

                                //save the file
                                actorPic.SaveAs(path);

                                //if these are all successful then we can set these fields
                                haspic = true;
                                picextension = extension;

                                //Update the actor haspic and picextension fields in the database
                                Actor Selectedactor = db.actors.Find(id);
                                Selectedactor.ActorHasPic = haspic;
                                Selectedactor.PicExtension = extension;
                                db.Entry(Selectedactor).State = EntityState.Modified;

                                db.SaveChanges();

                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine("actor Image was not saved successfully.");
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
        /// Add new actor information
        /// </summary>
        /// <param name="actor">actor entity</param>
        /// <returns>add actor information into database</returns>
        // POST: api/ActorData/AddActor
        [ResponseType(typeof(Actor))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult AddActor(Actor actor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.actors.Add(actor);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = actor.ActorID }, actor);
        }

        /// <summary>
        /// Delete selected actor
        /// </summary>
        /// <param name="id">passing parameter actor id</param>
        /// <returns>delete actor into database</returns>
        // POST: api/ActorData/DeleteActor/5
        [ResponseType(typeof(Actor))]
        [HttpPost]
        [Authorize]
        public IHttpActionResult DeleteActor(int id)
        {
            Actor Actor = db.actors.Find(id);
            if (Actor == null)
            {
                return NotFound();
            }

            db.actors.Remove(Actor);
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

        private bool ActorExists(int id)
        {
            return db.actors.Count(e => e.ActorID == id) > 0;
        }
    }
}