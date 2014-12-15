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

    public class StudentPlacementsController : ApiController
    {
        private Model db = new Model();

        // GET: api/StudentPlacements
        [Route("api/GetStudentPlacements")]        

        public IQueryable<StudentPlacement> GetStudentPlacements()
        {
            return db.StudentPlacements;
        }

        // GET: api/StudentPlacements/5
        [ResponseType(typeof(StudentPlacement))]
        [Route("api/GetStudentPlacement/{id:int}")]

        public async Task<IHttpActionResult> GetStudentPlacement(int id)
        {
            StudentPlacement studentPlacement = await db.StudentPlacements.FindAsync(id);
            if (studentPlacement == null)
            {
                return NotFound();
            }

            return Ok(studentPlacement);
        }

        // PUT: api/StudentPlacements/5
        [ResponseType(typeof(void))]
        [Route("api/PutStudentPlacement/{id:int}")]

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
        [Route("api/PostStudentPlacement")]

        public async Task<IHttpActionResult> PostStudentPlacement(StudentPlacement studentPlacement)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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

            return CreatedAtRoute("DefaultApi", new { id = studentPlacement.SPID }, studentPlacement);
        }

        // DELETE: api/StudentPlacements/5
        [ResponseType(typeof(StudentPlacement))]
        [Route("api/DeleteStudentPlacement/{id:int}")]
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
    }
}