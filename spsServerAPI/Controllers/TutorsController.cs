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
using Microsoft.AspNet.Identity;

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
                        Visits = t.tutorVisits.Select(visits
                            => new
                            {
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
                             join plc in db.Placements
                              on t.TutorID equals plc.AssignedTutorID
                             join p in db.StudentPreferences
                             on plc.PlacementID equals p.PID
                             join pp in db.PlacementProviders
                             on plc.ProviderID equals pp.ProviderID
                             where t.TutorID == id
                             select new
                             {
                                 t.TutorID,
                                 t.FirstName,
                                 t.SecondName,
                                 p.SID,
                                 p.PID,
                                 plc.StartDate,
                                 plc.FinishDate,
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
            var tutorView  = (from t in db.Tutors
                             join plc in db.Placements
                              on t.TutorID equals plc.AssignedTutorID
                             join p in db.StudentPreferences
                             on plc.PlacementID equals p.PID
                             join pp in db.PlacementProviders
                             on plc.ProviderID equals pp.ProviderID
                             select new
                             {
                                 t.TutorID,
                                 t.FirstName,
                                 t.SecondName,
                                 p.SID,
                                 p.PID,
                                 plc.StartDate,
                                 plc.FinishDate,
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
        [HttpPost]
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
                // to create an account for this tutor
                bool success = await MakeTutorAccount(tutor);
                if (!success)
                {
                    return BadRequest("Error Adding User login while adding Tutor " );
                }
                await db.SaveChangesAsync();

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

            try
            {
                using (ApplicationDbContext autDb = new ApplicationDbContext())
                {
                    ApplicationUser user = await autDb.Users
                        .FirstOrDefaultAsync(s => s.UserName == tutor.Email);
                    if (user == null)
                        return BadRequest("No login account found for " + tutor.Email);
                    // should cascade deletes of roles as well
                    autDb.Users.Remove(user);
                    await autDb.SaveChangesAsync();

                }
                db.Tutors.Remove(tutor);
                await db.SaveChangesAsync();
            }

            catch (System.Data.Entity.Validation.DbEntityValidationException e)
            {
                return BadRequest("Error Deleting Tutor " + e.Message);
            }

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

        private async Task<bool> MakeTutorAccount(Tutor t)
        {
            using (ApplicationDbContext autDb = new ApplicationDbContext())
            {
                var exists = await autDb.Users.SingleOrDefaultAsync(
                    s => s.UserName == t.Email);
                if (exists == null)
                {
                    ApplicationRole role = await autDb.Roles.FirstOrDefaultAsync(
                        r => r.Name == "tutor");
                    PasswordHasher p = new PasswordHasher();
                    var user = new ApplicationUser()
                    {
                        FirstName = t.FirstName,
                        SecondName = t.SecondName,
                        UserName = t.Email,
                        Email = t.Email,
                        Approved = true,
                        PasswordHash = p.HashPassword(t.Email),
                    };
                    autDb.Users.Add(user);
                    user.Roles.Add(new ApplicationUserRole { RoleId = role.Id });
                    try
                    {
                        await autDb.SaveChangesAsync();
                    }
                    catch (System.Data.Entity.Validation.DbEntityValidationException)
                    {
                        
                        return false;
                    }

                }

            }
            return true;
        }
    }
}