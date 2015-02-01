using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using spsServerAPI.Models;
using System.Web.Http.Cors;

namespace spsServerAPI.Controllers
{
    [AllowAnonymous]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/PlacementTypes")]
    public class PlacementTypesController : ApiController
    {
        private Model db = new Model();

        // GET: api/PlacementTypes
        [Route("GetPlacementTypes")]
        public dynamic GetPlacementTypes()
        {
            var placementTypes = (from pt in db.PlacementTypes
                                  select new { pt.Id, pt.Description });

            return placementTypes;
        }

        // GET: api/PlacementTypes/5
        [ResponseType(typeof(PlacementType))]
        [Route("GetPlacementType/{id:int}")]
        public async Task<dynamic> GetPlacementType(int id)
        {
            PlacementType placementType = await db.PlacementTypes.FindAsync(id);
            if (placementType == null)
            {
                return NotFound();
            }

            return Ok(
                (from pt in db.PlacementTypes
                                  where pt.Id == id
                                  select new { pt.Id, pt.Description })
                );
        }

        // PUT: api/PlacementTypes/5
        [ResponseType(typeof(void))]
        [Route("PutPlacementType/{id:int}")]
        public async Task<IHttpActionResult> PutPlacementType(int id, PlacementType placementType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != placementType.Id)
            {
                return BadRequest();
            }

            db.Entry(placementType).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlacementTypeExists(id))
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

        // POST: api/PlacementTypes
        [ResponseType(typeof(PlacementType))]
        [Route("PostPlacementType")]
        public async Task<IHttpActionResult> PostPlacementType(PlacementType placementType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.PlacementTypes.Add(placementType);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PlacementTypeExists(placementType.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Ok(placementType);
        }

        // DELETE: PlacementTypes/5
        [ResponseType(typeof(PlacementType))]
        [Route("DeletePlacementType/{id:int}")]
        public async Task<IHttpActionResult> DeletePlacementType(int id)
        {
            PlacementType placementType = await db.PlacementTypes.FindAsync(id);
            if (placementType == null)
            {
                return NotFound();
            }

            db.PlacementTypes.Remove(placementType);
            await db.SaveChangesAsync();

            return Ok(placementType);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PlacementTypeExists(int id)
        {
            return db.PlacementTypes.Count(e => e.Id == id) > 0;
        }
    }
}