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


        //get: GetStudentPlacementByStudentID
        [ResponseType(typeof(List<StudentPlacement>))]
        [Route("GetStudentPlacementByStudentID/{id:regex(^([a-zA-Z0-9_\\-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([a-zA-Z0-9\\-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$)}")]

        public async Task<IHttpActionResult> GetStudentPlacementByStudentID(string id)
        {
            var studentPlacements = db.StudentPlacements
                                        .OrderBy(sp => sp.SPID)
                                            .Where(sp => sp.SID == id)
                                            .Select(sp => sp);
            if (studentPlacements.Count() == 0)
            {
                return NotFound();
            }
            return Ok(studentPlacements.ToList());
        }

        // GET: api/StudentPlacementsByPlacementId/5
        [ResponseType(typeof(List<StudentPlacement>))]
        [Route("GetStudentPlacementsByPlacementId/{id:int}")]
        public dynamic GetStudentPlacementsByPlacementId(int id)
        {
            var studentPlacements = db.StudentPlacements.Include("Placements").Where(sp => sp.PlacementID == id);
            if (studentPlacements.Count() == 0)
            {
                return NotFound();
            }

            return Ok(studentPlacements.ToList());
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