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
            return db.PlacedStudents.Where(p => p.Year == year);
        }

        [Route("GetStudentPlaced/PID/{pid:int}")]
        public dynamic GetStudentPlaced(int pid)
        {

            var ret = (from placed in db.PlacedStudents
                       join p in db.Placements
                       on placed.PID equals p.PlacementID
                       join s in db.Students
                       on placed.SID equals s.SID
                       join sprog in db.StudentProgrammeStages
                       on s.SID equals sprog.SID
                       join progStage in db.ProgrammeStages
                       on sprog.ProgrammeStageID equals progStage.Id
                       where placed.PID == pid 
                       select new 
                       {
                       SID = placed.SID,
                       Name = String.Concat(new string[] {s.FirstName," ",s.SecondName}),
                       Pref = 0,
                       Prog = String.Concat(new string[] { progStage.ProgrammeCode, " Stage ", progStage.Stage.ToString() }),
                       PlcDesc = p.PlacementDescription
                       }
                           ).Distinct();

            return ret;
        }


        [HttpPost]
        [Route("PostPlaced/PID/{pid:int}/SID/{sid:regex(^([a-zA-Z0-9_\\-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([a-zA-Z0-9\\-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$)}")]
        public dynamic PostPlaced(int pid, string sid)
        {
            // This will need to be done using constraints;
            var placement = db.Placements.Find(pid);
            var student = db.Students.Find(sid);
            var exists = db.PlacedStudents.Where(p => p.SID == sid);

            if(student == null)
                return BadRequest("Student ID " + sid + " does not exist to be placed");
            if (placement == null)
                return BadRequest("Placement ID" + pid.ToString() + " does not exist ");
            if (exists.Count() > 0)
                return BadRequest("Student ID" + sid.ToString() + " already placed ");

            Placed placed = new Placed { PID = pid, SID = sid };
            db.PlacedStudents.Add(placed);
            placement.Filled = true;

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
        [Route("DeletePlaced/PID/{pid:int}")]
        public async Task<IHttpActionResult> DeletePlaced(int pid)
        {
            Placed placed = db.PlacedStudents
                .SingleOrDefault(e => e.PID == pid );
            if (placed == null)
            {
                return BadRequest("No Record to delete for " + pid.ToString());
            }
             //this is probably redundant now but for cosistancy

            Placement placement = db.Placements
                .SingleOrDefault(e => e.PlacementID == pid );
            
            if(placement == null)
            {
                return NotFound();
            }
            placement.Filled = false;

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
