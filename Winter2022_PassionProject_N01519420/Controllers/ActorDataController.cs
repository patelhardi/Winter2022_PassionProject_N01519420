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
                ActorLastName = a.ActorLastName
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
        // GET: api/MovieData/ListActorsForMovie/1
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
                ActorLastName = Actor.ActorLastName
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
        /// Add new actor information
        /// </summary>
        /// <param name="actor">actor entity</param>
        /// <returns>add actor information into database</returns>
        // POST: api/ActorData/AddActor
        [ResponseType(typeof(Actor))]
        [HttpPost]
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