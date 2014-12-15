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

    public class StudentsController : ApiController
    {
        private Model db = new Model();

        // GET: api/Students
        [Route("api/GetStudents")]
        public IQueryable<Student> GetStudents()
        {
            return db.Students;
        }

        // GET: api/Students/5
        [Route("api/GetStudent/{id:int}")]
        [ResponseType(typeof(Student))]
        public async Task<IHttpActionResult> GetStudent(string id)
        {
            Student student = await db.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            return Ok(student);
        }

        // PUT: api/Students/5
        [ResponseType(typeof(void))]
        [Route("api/PutStudent/{id:int}")]
        public async Task<IHttpActionResult> PutStudent(string id, Student student)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != student.SID)
            {
                return BadRequest();
            }

            db.Entry(student).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(id))
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

        // POST: api/Students
        [ResponseType(typeof(Student))]
        [Route("api/PostStudent")]
        public async Task<IHttpActionResult> PostStudent(Student student)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Students.Add(student);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (StudentExists(student.SID))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Ok(student);
        }

        // DELETE: api/Students/5
        [ResponseType(typeof(Student))]
        [Route("api/DeleteStudent/{id:int}")]
        public async Task<IHttpActionResult> DeleteStudent(string id)
        {
            Student student = await db.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }

            db.Students.Remove(student);
            await db.SaveChangesAsync();

            return Ok(student);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool StudentExists(string id)
        {
            return db.Students.Count(e => e.SID == id) > 0;
        }
    }
}