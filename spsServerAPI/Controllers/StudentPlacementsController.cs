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


        [Route("GetStudentsWithPreferencesForPlacement/PID/{pid:int}")]
        public dynamic GetStudentsWithPreferencesForPlacement(int pid)
        {

            var ret = ( from s in db.Students
                        join preferenced in db.StudentPreferences
                        on s.SID equals preferenced.SID
                        join sprog in db.StudentProgrammeStages
                        on s.SID equals sprog.student.SID
                        join progStage in db.ProgrammeStages
                        on sprog.ProgrammeStageID equals progStage.Id
                        join p in db.Placements
                        on preferenced.PID equals p.PlacementID
                       where preferenced.PID== pid 
                       select new
                       {
                           SID = preferenced.SID,
                           Name = String.Concat(new string[] { s.FirstName, " ", s.SecondName }),
                           Pref = preferenced.Preference,
                           Prog = String.Concat(new string[] { progStage.ProgrammeCode, " Stage ", progStage.Stage.ToString() }),
                           PlcDesc = p.PlacementDescription
                       }
                           ).Distinct();
            
            //var placedIds = db.PlacedStudents
            //    .Join(db.StudentPreferences,plc => plc.PID, pref => pref.PID,
            //            (sp1,pref) => new {sp1,pref})
            //    .Select(x => x.pref.SID);
            var placed = new HashSet<string>(db.Placements.Select( x => x.AssignedStudentID));
            var others = ret.Where(a => !placed.Contains(a.SID));

            return others;
        }

        [Route("GetStudentsWithNoPreferences/PID/{pid:int}")]
        public dynamic GetStudentsWithNoPreferences(int pid)
        {
            //var placedIds = db.PlacedStudents
            //    .Join(db.StudentPreferences, plc => plc.PID, pref => pref.PID,
            //            (sp1, pref) => new { sp1, pref })
            //    .Select(x => x.sp1.SID);
            var placedIds = new HashSet<string>(db.Placements.Select(x => x.AssignedStudentID));

            var nonPlaced = db.Students.Where(s => !placedIds.Contains(s.SID)).Select(s => s.SID).Distinct();
            var studentsWithPlacements = db.StudentPreferences.Select(s => s.SID).Distinct();
            var others = nonPlaced.Except(studentsWithPlacements);
            

            var result = db.Students.Where(s => others.Contains(s.SID))
                 .Join(db.StudentProgrammeStages, sp2 => sp2.SID, sps => sps.SID,
                 (sp2, sps) => new { sp2, sps })
                 .Join(db.ProgrammeStages, ps => ps.sps.ProgrammeStageID, prog => prog.Id,
                 (sp3, prog) => new { sp3, prog })
                 .Select(a => new
                 {
                     SID = a.sp3.sp2.SID,
                     Name = String.Concat(a.sp3.sp2.FirstName , " " , a.sp3.sp2.SecondName),
                     Pref = 99,
                     Prog = String.Concat(a.prog.ProgrammeCode, " stage ", a.prog.Stage.ToString())
                 });
            return result;
        }

        // GET: api/StudentPlacements
        [Route("GetStudentPlacements")]        
        public IQueryable<StudentPreference> GetStudentPlacements()
        {
            return db.StudentPreferences;
        }

        [Route("GetPreferencesByPlcId/PID/{pid:int}/Preference/{pref:int}")]
        public dynamic GetPreferencesByPlcId(int pid, int? pref)
        {
            var result = (from s in db.Students
                          join sp in db.StudentPreferences
                          on s.SID equals sp.SID
                          join p in db.Placements
                          on sp.PID equals p.PlacementID
                          join sps in db.StudentProgrammeStages
                          on s.SID equals sps.SID
                          join ps in db.ProgrammeStages
                          on sps.ProgrammeStageID equals ps.Id
                          where sp.PID == pid
                          && sp.Preference == pref
                          && p.StartDate.Value.Year == sps.Year

                          select new
                          {
                              s.SID,
                              s.FirstName,
                              s.SecondName,
                              sp.PID,
                              sp.Status,
                              ps.ProgrammeCode,
                              ps.Stage,
                              sps.Year
                          });
            //if (result.Count() == 0)
            //{
            //    return BadRequest("No matching records for" + pid.ToString() + " and " + pref.ToString());
            //}

            return Ok(result);

        }

        [Route("GetPreferencesBySIDandYear/SID/{sid:regex(^([a-zA-Z0-9_\\-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([a-zA-Z0-9\\-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$)}/Year/{year:int}")]
        public dynamic GetPreferencesBySIDandYear(string sid, int year)
        {
            var result = (from s in db.Students
                          join sp in db.StudentPreferences
                          on s.SID equals sp.SID
                          join p in db.Placements
                          on sp.PID equals p.PlacementID
                          where s.SID == sid
                          && p.StartDate.Value.Year == year
                          select new
                          {
                              sp.PID,
                              sp.Preference,
                              sp.Status,
                              p.PlacementID,
                              p.PlacementDescription
                          });
            //if (result.Count() == 0)
            //{
            //    return BadRequest("No matching records for" + sid.ToString() + " and " + year.ToString());
            //}

            return Ok(result);

        }

        //get: GetStudentPlacementByStudentIDandYear
        [ResponseType(typeof(List<StudentPreference>))]
        [Route("GetStudentPlacementByStudentIDandYear/SID/{id:regex(^([a-zA-Z0-9_\\-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([a-zA-Z0-9\\-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$)}/Year/{year:int}")]

        public dynamic GetStudentPlacementByStudentIDandYear(string id, int year)
        {
            var studentPlacements = db.StudentPreferences
                                        .Include(p => p.placement)
                                        .Include(s => s.student)
                                        .OrderBy(sp => sp.PID)
                                            .Where(sp => sp.SID == id)
                                            .Where(sp => sp.placement.FinishDate.Value.Year == year)
                                            .Select(sp => new {
                                                sp.PID,
                                                sp.SID,
                                                sp.student.FirstName,
                                                sp.student.SecondName,
                                                sp.Status,
                                                sp.Preference,
                                                sp.placement.PlacementDescription,
                                                sp.placement.StartDate,
                                                sp.placement.FinishDate
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
            var studentPlacements = db.StudentPreferences
                .Where(sp => sp.PID == id).Where(sp => sp.placement.StartDate.Value.Year == year);
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
            var studentPlacementWithNames = db.StudentPreferences
                .Join(db.Students, sp => sp.SID, s => s.SID,
                (sp2, sps) => new { sp2, sps })
                .Where(sp => sp.sp2.PID == id)
                .Select(a => new
                {
                    PID = a.sp2.PID,
                    SID = a.sp2.SID,
                    PlacementID = a.sp2.PID,
                    Preference = a.sp2.Preference,
                    Status = a.sp2.Status,
                    Name = String.Concat(a.sps.FirstName, " ", a.sps.SecondName),
                });
            if (studentPlacementWithNames.Count() == 0)
            {
                string message = "No Student Placements for PID " + id.ToString();
                return NotFound(message);
            }
            return Ok(studentPlacementWithNames);
        }
        // GET: api/StudentPlacements/5
        [ResponseType(typeof(StudentPreference))]
        [Route("GetStudentPlacement/{id:int}")]

        public async Task<IHttpActionResult> GetStudentPlacement(int id)
        {
            StudentPreference studentPlacement = await db.StudentPreferences.FindAsync(id);
            if (studentPlacement == null)
            {
                return NotFound();
            }

            return Ok(studentPlacement);
        }

        [HttpPost]
        [Route("PostPreferenceSIDPID/SID/{sid:regex(^([a-zA-Z0-9_\\-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([a-zA-Z0-9\\-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$)}/PID/{pid:int}/Pref/{pref:int}")]
        
        public dynamic PostPreferenceSIDPID(string sid, int pid, int pref)
        {
            if (db.StudentPreferences.Where(p => p.SID == sid && p.PID == pid && p.Preference == pref)
                .Select(p => p).Count() > 0)
                return BadRequest("Preference already exists for this student placement");
            StudentPreference sp = new
                StudentPreference { SID = sid, PID = pid, Preference = pref, Status = 0, TimeStamp = DateTime.Now.Date };
            db.StudentPreferences.Add( sp);
            db.SaveChanges();
                return Ok(sp);
        }

        [Route("DeletePreferenceSIDPID/SID/{sid:regex(^([a-zA-Z0-9_\\-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([a-zA-Z0-9\\-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$)}/PID/{pid:int}/Pref/{pref:int}")]
        
        public dynamic DeletePreferenceSIDPID(string sid, int pid, int pref)
        {
            var sps = db.StudentPreferences.Where(p => p.SID == sid && p.PID == pid && p.Preference == pref)
                .Select(p => p);
            if (sps.Count() == 0)
                return NoContent("");
            StudentPreference toDelete = sps.First();
            db.StudentPreferences.Remove(toDelete);
            db.SaveChanges();
            return Ok(toDelete.PID.ToString() + " was deleted");
        }


        // PUT: api/StudentPlacements/5
        [ResponseType(typeof(void))]
        [Route("PutStudentPlacement/PID{Pid:int}/SID/{Sid:regex(^([a-zA-Z0-9_\\-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([a-zA-Z0-9\\-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$)}")]

        public async Task<IHttpActionResult> PutStudentPlacement(int Pid, string Sid,  StudentPreference studentPlacement)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (Pid != studentPlacement.PID && Sid != studentPlacement.SID)
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
                if (!StudentPlacementExists(Pid,Sid))
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

        [HttpPut]
        [Route("AssignStudentToPlacement/SID/{sid:regex(^([a-zA-Z0-9_\\-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([a-zA-Z0-9\\-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$)}/PID/{pid:int}/Pref/{pref:int}")]
        public dynamic AssignStudentToPlacement(string sid, int pid)
        {
            var aleardyThere = db.StudentPreferences.SingleOrDefault(sp => sp.SID == sid && sp.PID == pid);
            if (aleardyThere != null)
                return Duplicate("Student is already placed");
            else
            {
                
            }
            return Ok();
        }
        // POST: api/StudentPlacements
        [ResponseType(typeof(StudentPreference))]
        [Route("PostStudentPlacement")]

        public async Task<IHttpActionResult> PostStudentPlacement(StudentPreference studentPlacement)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            // Check for preferences exceeded
            var currentStudentPlacements = db.StudentPreferences.Select(sp => sp.SID == studentPlacement.SID);
            if (currentStudentPlacements.Count() > 2)
                throw BadRequest("Preferences cannot be more than " + 
                                    currentStudentPlacements.Count().ToString());

            db.StudentPreferences.Add(studentPlacement);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (StudentPlacementExists(studentPlacement.PID, studentPlacement.SID))
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
        [ResponseType(typeof(StudentPreference))]
        [Route("DeleteStudentPlacement/{id:int}")]
        public async Task<IHttpActionResult> DeleteStudentPlacement(int id)
        {
            StudentPreference studentPlacement = await db.StudentPreferences.FindAsync(id);
            if (studentPlacement == null)
            {
                return NotFound();
            }

            db.StudentPreferences.Remove(studentPlacement);
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

        private bool StudentPlacementExists(int Pid, string Sid )
        {
            return db.StudentPreferences.Count(e => e.PID == Pid && e.SID == Sid) > 0;
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

        protected HttpResponseException Duplicate(string reason)
        {
            return CreateHttpResponseException(reason, HttpStatusCode.Forbidden);
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