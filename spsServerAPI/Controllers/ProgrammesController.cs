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
using System.Web.Http.Routing;

namespace spsServerAPI.Controllers
{
    [AllowAnonymous]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/Programmes")]
    public class ProgrammesController : ApiController
    {
        private Model db = new Model();

        // GET: api/Programmes
        [Route("GetProgrammes")]        
        public IQueryable<Programme> GetProgrammes()
        {
            return db.Programmes;
        }

        //GetProgrammesRaw
        // GET: api/Programmes
        [Route("GetProgrammesRaw")]
        public dynamic GetProgrammesRaw()
        {
            return (from programme in db.Programmes
                    join Stages in db.ProgrammeStages
                        on programme.ProgrammeCode equals Stages.ProgrammeCode
                    select new
                    {
                        programme.ProgrammeCode,
                        programme.ProgrammeName
                        //Stage = programme.ProgrammeStages.Select(p => p.Stage)
                    }
                    ).Distinct();
 
        }

        [Route("GetProgrammeStages/{code}")]
        public dynamic GetProgrammesRaw(string code)
        {
            return (from programme in db.Programmes
                    join Stages in db.ProgrammeStages
                        on programme.ProgrammeCode equals Stages.ProgrammeCode
                        where programme.ProgrammeCode == code
                    select new
                    {
                        Stages.Stage
                    }
                    );

        }

        [Route("GetProgrammesListWithStages")]
        public dynamic GetProgrammesListWithStages()
        {
            return (from programmes in db.Programmes
                    join stages in db.ProgrammeStages
                on programmes.ProgrammeCode equals stages.ProgrammeCode
                    select new { programmes.ProgrammeCode, programmes.ProgrammeName,
                            stages.Id,stages.Stage});
                
        }

        [Route("GetProgrammesListPaged")]
        public IEnumerable<Programme> GetProgrammesList(int page = 0, int pageSize = 10)
        {
            IQueryable<Programme> query;
            query = db.Programmes.Select(p => p).OrderBy(p=>p.ProgrammeCode);
            var totalCount = query.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            var urlHelper = new UrlHelper(Request);
            var prevLink = page > 0 ? urlHelper.Link("Programmes", new { page = page - 1, pageSize = pageSize }) : "";
            var nextLink = page < totalPages - 1 ? urlHelper.Link("Programmes", new { page = page + 1, pageSize = pageSize }) : "";

            var paginationHeader = new
            {
                TotalCount = totalCount,
                TotalPages = totalPages,
                PrevPageLink = prevLink,
                NextPageLink = nextLink
            };

            System.Web.HttpContext.Current.Response.Headers.Add("X-Pagination",
            Newtonsoft.Json.JsonConvert.SerializeObject(paginationHeader));

            var results = query
                    .Skip(pageSize * page)
                    .Take(pageSize).
                    ToList();

            return results;
        }

        // GET: api/Programmes/5
        [ResponseType(typeof(Programme))]
        [Route("GetProgramme/{name}")]
        public async Task<IHttpActionResult> GetProgramme(string name)
        {
            Programme programme = await db.Programmes.FindAsync(name);
            if (programme == null)
            {
                return NotFound();
            }

            return Ok(programme);
        }

        // PUT: api/Programmes/5
        [ResponseType(typeof(void))]
        [Route("PutProgramme/{name}")]
        public async Task<IHttpActionResult> PutProgramme(string name, Programme programme)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (name != programme.ProgrammeCode)
            {
                return BadRequest();
            }

            db.Entry(programme).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProgrammeExists(name))
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

        // POST: api/Programmes
        [ResponseType(typeof(Programme))]
        [Route("PostProgramme")]

        public async Task<IHttpActionResult> PostProgramme(Programme programme)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Programmes.Add(programme);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ProgrammeExists(programme.ProgrammeCode))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Ok(programme);
        }

        // DELETE: api/Programmes/5
        [ResponseType(typeof(Programme))]
        [Route("DeleteProgramme/{name}")]
        public async Task<IHttpActionResult> DeleteProgramme(string name)
        {
            Programme programme = await db.Programmes.FindAsync(name);
            if (programme == null)
            {
                return NotFound();
            }

            db.Programmes.Remove(programme);
            await db.SaveChangesAsync();

            return Ok(programme);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProgrammeExists(string id)
        {
            return db.Programmes.Count(e => e.ProgrammeCode == id) > 0;
        }
    }
}