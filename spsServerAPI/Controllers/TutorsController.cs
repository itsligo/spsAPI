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

    public class TutorsController : ApiController
    {
        private Model db = new Model();

        // GET: api/Tutors
        [Route("api/GetTutors")]
        public IQueryable<Tutor> GetTutors()
        {
            return db.Tutors;
        }

        // GET: api/Tutors/5
        [ResponseType(typeof(Tutor))]
        [Route("api/GetTutor/{id:int}")]
        public async Task<IHttpActionResult> GetTutor(int id)
        {
            Tutor tutor = await db.Tutors.FindAsync(id);
            if (tutor == null)
            {
                return NotFound();
            }

            return Ok(tutor);
        }

        // PUT: api/Tutors/5
        [ResponseType(typeof(void))]
        [Route("api/PutTutor/{id:int}")]
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
        [Route("api/PostTutors")]
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
        [Route("api/DeleteTutor/{id:int}")]
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