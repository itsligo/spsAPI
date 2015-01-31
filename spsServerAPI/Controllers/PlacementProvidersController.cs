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
using spsServerAPI.Models;
using System.Web.Http.Cors;

namespace spsServerAPI.Controllers
{

    [AllowAnonymous]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/PlacementProviders")]
    
    public class PlacementProvidersController : ApiController
    {
        private Model db = new Model();

        // GET: api/PlacementProviders
        [Route("GetPlacementProviders")]
        public dynamic GetPlacementProviders()
        {
            return db.PlacementProviders;
        }

        // GET: api/PlacementProviders
        [Route("GetPlacementProvidersList")]
        public dynamic GetPlacementProvidersList()
        {
            return (from pp in db.PlacementProviders
                    select new
                    {
                        pp.ProviderID,
                        pp.ProviderName,
                        pp.ProviderDescription
                    });
        }


        // GET: api/PlacementProviders/5
        [Route("PlacementProvider/{id:int}")]
        [ResponseType(typeof(PlacementProvider))]
        public IHttpActionResult GetPlacementProvider(int id)
        {
            PlacementProvider placementProvider = db.PlacementProviders.Find(id);
            if (placementProvider == null)
            {
                return NotFound();
            }

            return Ok(placementProvider);
        }

        [Route("PlacementProviders/{pname:alpha}")]
        [ResponseType(typeof(List<PlacementProvider>))]
        public IHttpActionResult GetPlacementProvider(string pname)
        {
            IQueryable<PlacementProvider> placementProviders = db.PlacementProviders.Where(p => p.ProviderName.Contains(pname));
            if (placementProviders == null)
            {
                return NotFound();
            }

            return Ok(placementProviders.ToList<PlacementProvider>());
        }

        // PUT: api/PlacementProvider/5
        [ResponseType(typeof(void))]
        [Route("PutPlacementProvider/{id:int}")]
        public IHttpActionResult PutPlacementProvider(int id, PlacementProvider placementProvider)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != placementProvider.ProviderID)
            {
                return BadRequest();
            }

            db.Entry(placementProvider).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlacementProviderExists(id))
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

        // POST: api/PlacementProviders
        [ResponseType(typeof(PlacementProvider))]
        [Route("PostPlacementProvider")]
        public IHttpActionResult PostPlacementProvider(PlacementProvider placementProvider)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.PlacementProviders.Add(placementProvider);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (PlacementProviderExists(placementProvider.ProviderID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            //return CreatedAtRoute("DefaultApi", new { id = placementProvider.ProviderID }, placementProvider);
            return Ok(placementProvider);
        }

        // DELETE: api/PlacementProviders/5
        [ResponseType(typeof(PlacementProvider))]
        [Route("DeletePlacementProvider/{id:int}")]
        public IHttpActionResult DeletePlacementProvider(int id)
        {
            PlacementProvider placementProvider = db.PlacementProviders.Find(id);
            if (placementProvider == null)
            {
                return NotFound();
            }

            db.PlacementProviders.Remove(placementProvider);
            db.SaveChanges();

            return Ok(placementProvider);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PlacementProviderExists(int id)
        {
            return db.PlacementProviders.Count(e => e.ProviderID == id) > 0;
        }
    }
}