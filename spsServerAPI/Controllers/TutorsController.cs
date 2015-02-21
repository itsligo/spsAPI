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
    [RoutePrefix("api/Tutors")]

    public class TutorsController : ApiController
    {
        private Model db = new Model();

        // GET: api/Tutors
        [Route("GetTutors")]
        public dynamic GetTutors()
        {
            return db.Tutors.Select(t => new 
            {
                t.TutorID,
                t.FirstName,
                t.SecondName,
                t.ContactNumber1,
                t.ContactNumber2
            });
        }



        // GET: api/Tutors/5
        [ResponseType(typeof(Tutor))]
        [Route("GetTutor/{id:int}")]
        public async Task<IHttpActionResult> GetTutor(int id)
        {
            Tutor tutor = await db.Tutors.FindAsync(id);
            if (tutor == null)
            {
                return NotFound();
            }

            return Ok(db.Tutors.Where(t => t.TutorID == id).Select(t => new 
            {
                t.TutorID,
                t.FirstName,
                t.SecondName,
                t.ContactNumber1,
                t.ContactNumber2
            }));
        }

        [Route("GetTutorVisits/{id:int}")]
        public async Task<IHttpActionResult> GetTutorVisits(int id)
        {
            Tutor tutor = await db.Tutors.FindAsync(id);
            if (tutor == null)
            {
                return NotFound();
            }

            return Ok(db.Tutors.Include("TutorVisit")
                .Where(t => t.TutorID == id)
                .Select(t => new 
                    {
                        t.TutorID,
                        t.FirstName,
                        t.SecondName,
                        Visits = t.TutorVisits.Select(visits 
                            => new {
                                visits.VisitID,
                                visits.DateVisited,
                                visits.Comment
                            })
                    }));
        }

        [Route("GetTutorAssignedPlacementsForTutorView/{id:int}")]
        public async Task<IHttpActionResult> GetTutorAssignedPlacementsForTutorView(int id)
        {
            Tutor tutor = await db.Tutors.FindAsync(id);
            if (tutor == null)
            {
                return NotFound();
            }

            var tutorView = (from t in db.Tutors
                            join sp in db.StudentPlacements
                             on t.TutorID equals sp.TutorID
                             join p in db.Placements
                             on sp.PlacementID equals p.PlacementID
                             join pp in db.PlacementProviders
                             on p.ProviderID equals pp.ProviderID
                             where t.TutorID == id
                             select new {
                                        t.TutorID,
                                        t.FirstName,
                                        t.SecondName,
                                        sp.SID,
                                        p.PlacementID,
                                        p.StartDate,
                                        p.FinishDate,
                                        pp.ProviderDescription,
                                        pp.AddressLine1,
                                        pp.AddressLine2,
                                        pp.County,
                                        pp.City,
                                        pp.ContactNumber
                                        }
                             );

            return Ok(tutorView);
        }

        [Route("GetTutorAssignedPlacementsForAdminView")]
        public async Task<IHttpActionResult> GetTutorAssignedPlacementsForAdminView()
        {
            var tutorView = (from t in db.Tutors
                             join sp in db.StudentPlacements
                              on t.TutorID equals sp.TutorID
                             join p in db.Placements
                             on sp.PlacementID equals p.PlacementID
                             join pp in db.PlacementProviders
                             on p.ProviderID equals pp.ProviderID
                             select new
                             {
                                 t.TutorID,
                                 t.FirstName,
                                 t.SecondName,
                                 sp.SID,
                                 p.PlacementID,
                                 p.StartDate,
                                 p.FinishDate,
                                 pp.ProviderDescription,
                                 pp.AddressLine1,
                                 pp.AddressLine2,
                                 pp.County,
                                 pp.City,
                                 pp.ContactNumber
                             }
                             );

            return Ok(tutorView);
        }
        // PUT: api/Tutors/5
        [ResponseType(typeof(void))]
        [Route("PutTutor/{id:int}")]
        public async Task<IHttpActionResult> PutTutor(int id, Tutor tutor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tutor.TutorID)
            {
                return BadRequest();
            }

            db.Entry(tutor).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TutorExists(id))
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

        // POST: api/Tutors
        [ResponseType(typeof(Tutor))]
        [Route("PostTutors")]
        public async Task<IHttpActionResult> PostTutor(Tutor tutor)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Tutors.Add(tutor);

            try
            {
                await db.SaveChangesAsync();
                //AccountController accMan = new AccountController();
                //await accMan.Register(new RegisterBindingModel { Fname = tutor.FirstName });
            }
            catch (DbUpdateException)
            {
                if (TutorExists(tutor.TutorID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Ok(tutor);
        }

        // DELETE: api/Tutors/5
        [ResponseType(typeof(Tutor))]
        [Route("DeleteTutor/{id:int}")]
        public async Task<IHttpActionResult> DeleteTutor(int id)
        {
            Tutor tutor = await db.Tutors.FindAsync(id);
            if (tutor == null)
            {
                return NotFound();
            }

            db.Tutors.Remove(tutor);
            await db.SaveChangesAsync();

            return Ok(tutor);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TutorExists(int id)
        {
            return db.Tutors.Count(e => e.TutorID == id) > 0;
        }
    }
}