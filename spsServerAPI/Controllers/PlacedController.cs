using spsServerAPI.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;

namespace spsServerAPI.Controllers
{
    [AllowAnonymous]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/Placed")]

    public class PlacedController : ApiController
    {
        Model db = new Model();

        [Route("GetPlaced")]
        public dynamic GetPlaced(int year)
        {
                var placedByYear = db.PlacedStudents
                .Join(db.Placements, sps => sps.PID, plc => plc.PlacementID,
                        (sp1, plc) => new { sp1, plc })
                .Where(result => result.plc.StartDate.Value.Year == year);

                return placedByYear;
        }

        [Route("GetStudentPlaced/PID/{pid:int}")]
        public dynamic GetStudentPlaced(int pid)
        {

            var ret = (from placed in db.PlacedStudents
                       join preference in db.StudentPreferences
                       on placed.PID equals preference.PID
                       join p in db.Placements
                       on preference.PID equals p.PlacementID
                       join s in db.Students
                       on preference.SID equals s.SID
                       join sprog in db.StudentProgrammeStages
                       on s.SID equals sprog.SID
                       join progStage in db.ProgrammeStages
                       on sprog.ProgrammeStageID equals progStage.Id
                       where placed.PID == pid 
                       select new 
                       {
                       SID = preference.SID,
                       Name = String.Concat(new string[] {s.FirstName," ",s.SecondName}),
                       Pref = 0,
                       Prog = String.Concat(new string[] { progStage.ProgrammeCode, " Stage ", progStage.Stage.ToString() }),
                       PlcDesc = p.PlacementDescription
                       }
                           ).Distinct();

            return ret;
        }

        // PUT: api/Tutors/5
        [ResponseType(typeof(void))]
        [Route("AssignTutorToPlacement/TID/{TutorID:int}/PID/{PlacementID:int}")]
        [HttpPut]
        public async Task<IHttpActionResult> AssignTutorToPlacement(int TID, int PID)
        {

            // May need to do calculation of assignments based on hours being allocated
            Placed p = await db.PlacedStudents.FindAsync(PID);
            Tutor t = await db.Tutors.FindAsync(TID);
            if (p == null)
                return NotFound();
            if (t == null)
                return NotFound();
            //p.TutorID = TID;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                return StatusCode(HttpStatusCode.BadRequest);
            }

            return StatusCode(HttpStatusCode.NoContent);
        }


        [HttpPost]
        [Route("PostPlaced/PID/{pid:int}/SID/{sid:regex(^([a-zA-Z0-9_\\-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([a-zA-Z0-9\\-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$)}")]
        public dynamic PostPlaced(int pid, string sid)
        {
            // This will need to be done using constraints;
            var placement = db.Placements.Find(pid);
            var student = db.Students.Find(sid);
            var exists = db.PlacedStudents
                .Join(db.StudentPreferences, plc => plc.PID, pref => pref.PID,
                        (sp1, pref) => new { sp1, pref })
                .Where(x => x.pref.SID == sid);
            
            //var exists = db.PlacedStudents.Where(p => p.SID == sid);

            if(student == null)
                return BadRequest("Student ID " + sid + " does not exist to be placed");
            if (placement == null)
                return BadRequest("Placement ID" + pid.ToString() + " does not exist ");
            if (exists.Count() > 0)
                return BadRequest("Student ID" + sid.ToString() + " already placed ");
            // Join placement and preferences to get the start date year for placed
            var prefjoin = db.StudentPreferences.Join(db.Placements,
                                    pref => pref.PID, plc => plc.PlacementID,
                                    (spref, place) =>
                                            new
                                            {
                                                spref.PID,
                                                spref.SID,
                                            });
            var preference = prefjoin.Select(p => new { p.PID, p.SID }).First();

            Placed placed = new Placed { PID = preference.PID, SID = preference.SID  };
            db.PlacedStudents.Add(placed);
            //placement.Filled = true;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (PlacedExists(placed))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Ok(placed);

        }

        [ResponseType(typeof(Placed))]
        [HttpDelete]
        [Route("DeletePlacedbyPID/pid/{pid:int}")]
        public async Task<IHttpActionResult> DeletePlacedbyPID(int pid)
        {
            Placed PlacedStudent = (Placed)(from placedStudent in db.PlacedStudents
                                                     join studentPreference in db.StudentPreferences
                                                     on placedStudent.PID equals studentPreference.PID
                                                     join placement in db.Placements
                                                     on studentPreference.PID equals placement.PlacementID
                                                     where placement.PlacementID == pid
                                                     select placedStudent).FirstOrDefault();
            if (PlacedStudent == null)
            {
                return BadRequest("No Record to delete for " + pid.ToString());
            }

            //int placedId = PlacedStudent.PID;
            //Placement studentPlacement = (Placement)(from placedStudent in db.PlacedStudents
            //                                         join studentPreference in db.StudentPreferences
            //                                         on placedStudent.PID equals studentPreference.PID
            //                                         join placement in db.Placements
            //                                         on studentPreference.PID equals placement.PlacementID
            //                                         where placedStudent.PID == placedId
            //                                         select placement);

            //studentPlacement.Filled = false;
            db.PlacedStudents.Remove(PlacedStudent);

            return Ok(PlacedStudent);
        }



        [ResponseType(typeof(Placed))]
        [HttpDelete]
        [Route("DeletePlaced/id/{id:int}")]
        public async Task<IHttpActionResult> DeletePlaced(int id)
        {
            Placed placed = db.PlacedStudents
                .SingleOrDefault(e => e.PID == id );
            if (placed == null)
            {
                return BadRequest("No Record to delete for " + id.ToString());
            }

            //Placement studentPlacement = (Placement)(from placedStudent in db.PlacedStudents
            //                 join studentPreference in db.StudentPreferences
            //                 on placedStudent.PID equals studentPreference.PID
            //                 join placement in db.Placements
            //                 on studentPreference.PID equals placement.PlacementID
            //                 where placedStudent.PID == id
            //                 select placement);
            ////studentPlacement.Filled = false;
            db.PlacedStudents.Remove(placed);
            await db.SaveChangesAsync();

            return Ok(placed);
        }

        private bool PlacedExists(Placed p)
        {
            return db.PlacedStudents.Count(
                e => e.PID == p.PID ) > 0;
 
        }
    }
}
