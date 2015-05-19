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
            var placedByYear = db.Placements
                .Where(p => p.AssignedStudentID != null && p.StartDate.Value.Year == year);
                //.Join(db.Placements, sps => sps.PID, plc => plc.PlacementID,
                //        (sp1, plc) => new { sp1, plc })
                //.Where(result => result.plc.StartDate.Value.Year == year);

                return placedByYear;
        }

        [Route("GetStudentPlaced/PID/{pid:int}")]
        public dynamic GetStudentPlaced(int pid)
        {
            var ret = 
                    //(from placed in db.PlacedStudents
                    //   join preference in db.StudentPreferences
                    //   on placed.PID equals preference.PID
                    //   join p in db.Placements
                    //   on preference.PID equals p.PlacementID
                    //   join s in db.Students
                    //   on preference.SID equals s.SID
                    //   join sprog in db.StudentProgrammeStages
                    //   on s.SID equals sprog.SID
                    //   join progStage in db.ProgrammeStages
                    //   on sprog.ProgrammeStageID equals progStage.Id
                    //   where placed.PID == pid 
                    (from placement in db.Placements
                     join s in db.Students
                     on placement.AssignedStudentID equals s.SID
                     join sprog in db.StudentProgrammeStages
                     on s.SID equals sprog.SID
                     join progStage in db.ProgrammeStages
                     on sprog.ProgrammeStageID equals progStage.Id
                     where placement.PlacementID == pid 
                       select new 
                       {
                       SID = placement.AssignedStudentID,
                       Name = String.Concat(new string[] {s.FirstName," ",s.SecondName}),
                       Pref = 0,
                       Prog = String.Concat(new string[] { progStage.ProgrammeCode, " Stage ", progStage.Stage.ToString() }),
                       PlcDesc = placement.PlacementDescription
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
            Placement p = await db.Placements.FindAsync(PID);
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
            var exists = db.Placements.Where(s => s.AssignedStudentID == sid);
                //.Join(db.StudentPreferences, plc => plc.PID, pref => pref.PID,
                //        (sp1, pref) => new { sp1, pref })
                //.Where(x => x.pref.SID == sid);
            
            //var exists = db.PlacedStudents.Where(p => p.SID == sid);

            if(student == null)
                return BadRequest("Student ID " + sid + " does not exist to be placed");
            if (placement == null)
                return BadRequest("Placement ID" + pid.ToString() + " does not exist ");
            if (exists.Count() > 0)
                return BadRequest("Student ID" + sid.ToString() + " already placed ");
            // Join placement and preferences to get the start date year for placed
            //var prefjoin = db.StudentPreferences.Join(db.Placements,
            //                        pref => pref.PID, plc => plc.PlacementID,
            //                        (spref, place) =>
            //                                new
            //                                {
            //                                    spref.PID,
            //                                    spref.SID,
            //                                });
            //var preference = prefjoin.Select(p => new { p.PID, p.SID }).First();

            //Placed placed = new Placed { PID = preference.PID, SID = preference.SID  };
            placement.AssignedStudentID = sid;
            //db.Placements.Add(placement);
            //placement.Filled = true;
            db.SaveChanges();

            return Ok(placement);

        }

        [ResponseType(typeof(Placement))]
        [HttpDelete]
        [Route("DeletePlacedbyPID/pid/{pid:int}")]
        public async Task<IHttpActionResult> DeletePlacedbyPID(int pid)
        {
            Placement PlacedStudent = (Placement)(from placedStudent in db.Placements
                                            where placedStudent.PlacementID == pid
                                            select placedStudent).FirstOrDefault();
            //if (PlacedStudent == null)
            //{
            //    return BadRequest("No Record to delete for " + pid.ToString());
            //}

            PlacedStudent.AssignedStudentID = null;
            db.SaveChanges();

            return Ok(PlacedStudent);
        }



        [ResponseType(typeof(Placement))]
        [HttpDelete]
        [Route("DeletePlaced/id/{id:int}")]
        public async Task<IHttpActionResult> DeletePlaced(int id)
        {
            Placement placed = db.Placements
                .SingleOrDefault(e => e.PlacementID == id);
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
            //db.PlacedStudents.Remove(placed);
            placed.AssignedStudentID = null;
            await db.SaveChangesAsync();

            return Ok(placed);
        }

        private bool PlacedExists(Placement p)
        {
            return db.Placements.Count(
                e => e.PlacementID == p.PlacementID) > 0;
 
        }
    }
}
