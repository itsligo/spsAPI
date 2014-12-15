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
using System.Threading.Tasks;

namespace spsServerAPI.Controllers
{
    [AllowAnonymous]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/Placements")]
    public class PlacementsController : ApiController
    {
        private Model db = new Model();

        // GET: api/Placements
        [Route("GetPlacements")]
        public dynamic GetPlacements()
        {
            var returned = db.Placements.Include("AllowablePlacements");
            return returned;
        }

        [Route("GetPlacementsAllowableForStudent/{studentId}")]
        public dynamic GetPlacementsAllowableForStudent(string studentId)
        {
            return (from p in db.Placements
                        join ap in db.AllowablePlacements
                        on p.PlacementID equals ap.PlacementID
                            join ps in db.ProgrammeStages
                             on ap.ProgrammeStageID equals ps.Id
                                join ss in db.StudentProgrammeStages
                                on ps.Id equals ss.ProgrammeStageID
                                where ss.SID == studentId
                                    select p
                                    );
        }

       [Route("GetAllowablePlacementsAvailableList")]
        public dynamic GetAllowablePlacementsList()
        {
            return db.Placements.Include("AllowablePlacements").Where(p => p.Filled == true);
        }

       [Route("GetPlacemenCountByProviderIDByYear/{year:int}/{providerId:int}")]
       public dynamic GetPlacementCountByProvider(int? year, int? providerId)
       {
           return db.Placements.Where(p => p.StartDate.Value.Year == year && p.ProviderID == providerId).Count();

       }

        [Route("GetAllowablePlacementsList/{placementId:int}")]
        public dynamic GetAllowablePlacementsList(int? placementId)
        {
            return db.AllowablePlacements.Include("ProgrammeStage").Include("Programme").Where(a => a.PlacementID == placementId).Select(a =>
                new { a.ProgrammeStage.Id, a.ProgrammeStage.Programme.ProgrammeName });
        }



        [Route("GetPlacementYearsList")]
        public dynamic GetPlacementYearList()
        {
            try
            {
                var yearList = (from p in db.Placements
                                where p.StartDate != null
                                select new
                                {
                                    id = p.StartDate.Value.Year,
                                    year = p.StartDate.Value.Year
                                }).Distinct();

                return yearList;
            }

            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        // GET: api/Placements/5
        [ResponseType(typeof(Placement))]
        [Route("GetPlacement/{id:int}")]
        public dynamic GetPlacement(int id)
        {
            Placement placement = db.Placements.Find(id);
            if (placement == null)
            {
                return NotFound();
            }
            Placement returnPlacement = db.Placements
                    .Include("AllowablePlacements")
                    .Include("PlacementProvider")
                    .Where(p => p.PlacementID == id).First();

            return Ok(returnPlacement);
        }

        // PUT: api/Placements/5
        [ResponseType(typeof(void))]
        [Route("PutPlacement/{id:int}")]
        public IHttpActionResult PutPlacement(int id, Placement placement)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != placement.PlacementID)
            {
                return BadRequest();
            }

            db.Entry(placement).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlacementExists(id))
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

        // POST: api/Placements
        [ResponseType(typeof(Placement))]
        [Route("PostPlacement")]
        public IHttpActionResult PostPlacement(Placement placement)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Placements.Add(placement);
            db.SaveChanges();

            return Ok(placement);
        }

        // DELETE: api/Placements/5
        [ResponseType(typeof(Placement))]
        [Route("DeletePlacement/{id:int}")]
        public IHttpActionResult DeletePlacement(int id)
        {
            Placement placement = db.Placements.Find(id);
            if (placement == null)
            {
                return NotFound();
            }

            db.Placements.Remove(placement);
            db.SaveChanges();

            return Ok(placement);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PlacementExists(int id)
        {
            return db.Placements.Count(e => e.PlacementID == id) > 0;
        }
    }
}