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
using System.Web.Http.Results;

namespace spsServerAPI.Controllers
{
    [AllowAnonymous]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/StudentPlacements")]
    public class StudentPlacementsController : ApiController
    {
        private Model db = new Model();

        // GET: api/StudentPlacements
        [Route("GetStudentPlacements")]        

        public IQueryable<StudentPlacement> GetStudentPlacements()
        {
            return db.StudentPlacements;
        }

        [Route("GetPreferencesByPlcId/PID/{pid:int}/Preference/{pref:int}")]
        public dynamic GetPreferencesByPlcId(int pid, int? pref)
        {
            var result = (from s in db.Students
                          join sp in db.StudentPlacements
                          on s.SID equals sp.SID
                          join p in db.Placements
                          on sp.PlacementID equals p.PlacementID
                          join sps in db.StudentProgrammeStages
                          on s.SID equals sps.SID
                          join ps in db.ProgrammeStages
                          on sps.ProgrammeStageID equals ps.Id
                          where sp.PlacementID == pid
                          && sp.Preference == pref
                          && p.StartDate.Value.Year == sps.Year

                          select new
                          {
                              s.SID,
                              s.FirstName,
                              s.SecondName,
                              sp.PlacementID,
                              sp.Status,
                              ps.ProgrammeCode,
                              ps.Stage,
                              sps.Year
                          });
            if (result.Count() == 0)
            {
                return BadRequest("No matching records for" + pid.ToString() + " and " + pref.ToString());
            }

            return Ok(result);

        }

        [Route("GetPreferencesBySIDandYear/SID/{sid:regex(^([a-zA-Z0-9_\\-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([a-zA-Z0-9\\-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$)}/Year/{year:int}")]
        public dynamic GetPreferencesBySIDandYear(string sid, int year)
        {
            var result = (from s in db.Students
                          join sp in db.StudentPlacements
                          on s.SID equals sp.SID
                          join p in db.Placements
                          on sp.PlacementID equals p.PlacementID
                          where s.SID == sid
                          && p.StartDate.Value.Year == year
                          select new
                          {
                              sp.SPID,
                              sp.Preference,
                              sp.Status,
                              p.PlacementID,
                              p.PlacementDescription
                          });
            if (result.Count() == 0)
            {
                return BadRequest("No matching records for" + sid.ToString() + " and " + year.ToString());
            }

            return Ok(result);

        }

        //get: GetStudentPlacementByStudentID
        [ResponseType(typeof(List<StudentPlacement>))]
        [Route("GetStudentPlacementByStudentID/SID/{id:regex(^([a-zA-Z0-9_\\-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([a-zA-Z0-9\\-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$)}/Year/{year:int}")]

        public dynamic GetStudentPlacementByStudentID(string id, int year)
        {
            var studentPlacements = db.StudentPlacements
                                        .Include("Placement")
                                        .Include("Student")
                                        .OrderBy(sp => sp.SPID)
                                            .Where(sp => sp.SID == id)
                                            .Where(sp => sp.Placement.FinishDate.Value.Year == year)
                                            .Select(sp => new {
                                                sp.SPID,
                                                sp.SID,
                                                sp.Student.FirstName,
                                                sp.Student.SecondName,
                                                sp.Status,
                                                sp.Preference,
                                                sp.Placement.PlacementDescription,
                                                sp.Placement.StartDate,
                                                sp.Placement.FinishDate
                                            });
            if (studentPlacements.Count() == 0)
            {
                return NotFound();
            }
            return Ok(studentPlacements);
        }

        // GET: api/StudentPlacementsByPlacementId/5
        //[ResponseType(typeof(List<StudentPlacement>))]
        [Route("GetStudentPlacementsByPlacementIdAndYear/PID/{id:int}/Year/{year:int}")]
        public dynamic GetStudentPlacementsByPlacementIdAndYear(int id, int year)
        {
            var studentPlacements = db.StudentPlacements
                .Where(sp => sp.PlacementID == id).Where(sp => sp.Placement.StartDate.Value.Year == year);
            if (studentPlacements.Count() == 0)
            {
                string message = "No Student Placement for PID " + id.ToString() + " and Year " + year.ToString();
                return NotFound(message);
            }

            return Ok(studentPlacements);
        }

        // GET: api/StudentPlacementsByPlacementId/5
        //[ResponseType(typeof(List<StudentPlacement>))]
        [Route("GetStudentPlacementsByPlacementId/{id:int}")]
        public dynamic GetStudentPlacementsByPlacementId(int id)
        {
            var studentPlacements = db.StudentPlacements
                .Where(sp => sp.PlacementID == id);
            if (studentPlacements.Count() == 0)
            {
                string message = "No Student Placement for PID " + id.ToString();
                return NotFound(message);
            }
            return Ok(studentPlacements);
        }
        // GET: api/StudentPlacements/5
        [ResponseType(typeof(StudentPlacement))]
        [Route("GetStudentPlacement/{id:int}")]

        public async Task<IHttpActionResult> GetStudentPlacement(int id)
        {
            StudentPlacement studentPlacement = await db.StudentPlacements.FindAsync(id);
            if (studentPlacement == null)
            {
                return NotFound();
            }

            return Ok(studentPlacement);
        }
        [Route("PostPreferenceSIDPID/SID/{sid:regex(^([a-zA-Z0-9_\\-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([a-zA-Z0-9\\-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$)}/PID/{pid:int}/Pref/{pref:int}")]
        
        public dynamic PostPreferenceSIDPID(string sid, int pid, int pref)
        {
            if (db.StudentPlacements.Where(p => p.SID == sid && p.PlacementID == pid && p.Preference == pref)
                .Select(p => p).Count() > 0)
                return BadRequest("Preference already exists for this student placement");
            StudentPlacement sp = new
                StudentPlacement { SID = sid, PlacementID = pid, Preference = pref, Status = 0 };
            db.StudentPlacements.Add( sp);
                db.SaveChanges();
                return Ok(sp);
        }

        [Route("DeletePreferenceSIDPID/SID/{sid:regex(^([a-zA-Z0-9_\\-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([a-zA-Z0-9\\-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$)}/PID/{pid:int}/Pref/{pref:int}")]
        
        public dynamic DeletePreferenceSIDPID(string sid, int pid, int pref)
        {
            var sps = db.StudentPlacements.Where(p => p.SID == sid && p.PlacementID == pid && p.Preference == pref)
                .Select(p => p);
            if (sps.Count() == 0)
                return NoContent("");
            StudentPlacement toDelete = sps.First();
            db.StudentPlacements.Remove(toDelete);
            db.SaveChanges();
            return Ok(toDelete.SPID.ToString() + " was deleted");
        }


        // PUT: api/StudentPlacements/5
        [ResponseType(typeof(void))]
        [Route("PutStudentPlacement/{id:int}")]

        public async Task<IHttpActionResult> PutStudentPlacement(int id, StudentPlacement studentPlacement)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != studentPlacement.SPID)
            {
                return BadRequest();
            }

            db.Entry(studentPlacement).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentPlacementExists(id))
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




        // POST: api/StudentPlacements
        [ResponseType(typeof(StudentPlacement))]
        [Route("PostStudentPlacement")]

        public async Task<IHttpActionResult> PostStudentPlacement(StudentPlacement studentPlacement)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            // Check for preferences exceeded
            var currentStudentPlacements = db.StudentPlacements.Select(sp => sp.SID == studentPlacement.SID);
            if (currentStudentPlacements.Count() > 2)
                throw BadRequest("Preferences cannot be more than " + 
                                    currentStudentPlacements.Count().ToString());

            db.StudentPlacements.Add(studentPlacement);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (StudentPlacementExists(studentPlacement.SPID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Ok(studentPlacement);
        }

        // DELETE: api/StudentPlacements/5
        [ResponseType(typeof(StudentPlacement))]
        [Route("DeleteStudentPlacement/{id:int}")]
        public async Task<IHttpActionResult> DeleteStudentPlacement(int id)
        {
            StudentPlacement studentPlacement = await db.StudentPlacements.FindAsync(id);
            if (studentPlacement == null)
            {
                return NotFound();
            }

            db.StudentPlacements.Remove(studentPlacement);
            await db.SaveChangesAsync();

            return Ok(studentPlacement);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool StudentPlacementExists(int id)
        {
            return db.StudentPlacements.Count(e => e.SPID == id) > 0;
        }

        /// <summary>
        /// creates an <see cref="HttpResponseException"/> with a response code of 400
        /// and places the reason in the reason header and the body.
        /// </summary>
        /// <param name="reason">Explanation text for the client.</param>
        /// <returns>A new HttpResponseException</returns>
        protected HttpResponseException BadRequest(string reason)
        {
            return CreateHttpResponseException(reason, HttpStatusCode.BadRequest);
        }

        protected HttpResponseException NoContent(string reason)
        {
            return CreateHttpResponseException(reason, HttpStatusCode.NoContent);
        }

        /// <summary>
        /// creates an <see cref="HttpResponseException"/> with a response code of 404
        /// and places the reason in the reason header and the body.
        /// </summary>
        /// <param name="reason">Explanation text for the client.</param>
        /// <returns>A new HttpResponseException</returns>
        protected HttpResponseException NotFound(string reason)
        {
            return CreateHttpResponseException(reason, HttpStatusCode.NotFound);
        }

        /// <summary>
        /// Creates an <see cref="HttpResponseException"/> to be thrown by the api.
        /// </summary>
        /// <param name="reason">Explanation text, also added to the body.</param>
        /// <param name="code">The HTTP status code.</param>
        /// <returns>A new <see cref="HttpResponseException"/></returns>
        private static HttpResponseException CreateHttpResponseException(string reason, HttpStatusCode code)
        {
            var response = new HttpResponseMessage
            {
                StatusCode = code,
                ReasonPhrase = reason,
                Content = new StringContent(reason)
            };
            throw new HttpResponseException(response);
        }
    }
}