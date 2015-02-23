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

        [Route("GetPlaced/{year:int}")]
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
        [Route("PostPlaced")]
        public dynamic PostPlaced(Placed placed)
        {
            // This will need to be done using constraints;
            var placement = db.Placements.Find(placed.PID);
            var student = db.Students.Find(placed.SID);
            if(student == null)
                return BadRequest("Student ID " + placed.SID + " does not exist ");
            if (placement == null)
                return BadRequest("Placement ID" + placed.PID+ " does not exist ");

            if (placed.Year == null)
                placed.Year = DateTime.Now.Year;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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
        [Route("DeletePlaced/SID/{sid:regex(^([a-zA-Z0-9_\\-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([a-zA-Z0-9\\-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$)}/PID/{pid:int}/Year/{year:int}")]
        public async Task<IHttpActionResult> DeletePlaced(string sid, int pid, int year)
        {
            Placed placed = db.PlacedStudents
                .SingleOrDefault(e => e.SID == sid && e.PID == pid && e.Year == year);
            if (placed == null)
            {
                return NotFound();
            }
            // this is probably redundant now but for cosistancy
            //Placement placement = db.Placements
            //    .SingleOrDefault(e => e.PlacementID == pid 
            //        && e.StartDate.Value.Year == year);
            
            //if(placement == null)
            //{
            //    return NotFound();
            //}
            //placement.Filled = true;

            db.PlacedStudents.Remove(placed);
            await db.SaveChangesAsync();

            return Ok(placed);
        }

        private bool PlacedExists(Placed p)
        {
            return db.PlacedStudents.Count(
                e => e.SID == p.SID && e.PID == p.PID && e.Year == p.Year) > 0;
 
        }
    }
}
