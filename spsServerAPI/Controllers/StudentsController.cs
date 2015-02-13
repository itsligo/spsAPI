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
    [RoutePrefix("api/Students")]
    public class StudentsController : ApiController
    {
        private Model db = new Model();

        // GET: api/Students
        [Route("GetStudents")]
        public IQueryable<Student> GetStudents()
        {
            return db.Students;
        }

        // GET: api/Students/5
        [Route("GetStudent/{id}")]
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

        // GET: api/Students/5
        [Route("GetStudentByIdForYear/SID/{id}/Year/{year:int}")]
        public dynamic GetStudentByIdForYear(string id, int year)
        {

            var result = (from s in db.Students
                          where s.SID == id
                          select new
                          {
                              s.SID,
                              s.FirstName,
                              s.SecondName,
                              StudentDetails =
                              (from sps in db.StudentProgrammeStages
                               join ps in db.ProgrammeStages
                               on sps.ProgrammeStageID equals ps.Id
                               join p in db.Programmes
                               on ps.ProgrammeCode equals p.ProgrammeCode
                               where sps.SID == id
                               where sps.Year == year
                               select new
                               {
                                   studentProgrammeStage = new
                                   {
                                       sps.MemberID,
                                       sps.Year,
                                       programmeStage =
                                           new
                                           {
                                               p.ProgrammeName,
                                               ps.Stage
                                           }
                                   }
                               }),
                          }
               );

        
            if (result.Count() == 0)
            {
                return NotFound();
            }
            
            return Ok(result);
        }

        [Route("GetStudentProgrammeStages/SID/{id}")]
        public dynamic GetStudentProgrammeStages(string id)
        {

            var result = (from s in db.Students
                          where s.SID == id
                          select new
                          {
                              s.SID,
                              s.FirstName,
                              s.SecondName,
                              StudentDetails =
                              (from sps in db.StudentProgrammeStages
                               join ps in db.ProgrammeStages
                               on sps.ProgrammeStageID equals ps.Id
                               join p in db.Programmes
                               on ps.ProgrammeCode equals p.ProgrammeCode
                               where sps.SID == id
                               select new
                               {
                                   studentProgrammeStage = new
                                   {
                                       sps.MemberID,
                                       sps.Year,
                                       programmeStage =
                                           new { 
                                               p.ProgrammeName, ps.Stage 
                                           }
                                   }
                               }),
                          }
               );


            if (result.Count() == 0)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [Route("GetStudentYearsList")]
        public dynamic GetStudentYearsList()
        {
            var yearList = (from p in db.StudentProgrammeStages
                            where p.Year != null
                            select new
                            {
                                id = p.Year,
                                year = p.Year
                            }).Distinct();
            return yearList;
        }

        //GetAllStudentsInYearWithPlacements/2016
        [Route("GetAllStudentsInYearWithPlacements/{year:int}")]
        public dynamic GetAllStudentsInYearWithPlacements(int year)
        {
            var StudentsforYear = (from s in db.Students
                                   join sps in db.StudentProgrammeStages
                                   on s.SID equals sps.SID
                                   join sp in db.StudentPlacements
                                   on sps.SID equals sp.SID
                            where sps.Year == year
                            select new
                            {
                                s.SID,
                                s.FirstName,
                                s.SecondName,
                                sps.ProgrammeStageID,
                                sp.Status
                            }).Distinct();
            return StudentsforYear;
        }

        //GetAllStudentsInYearWithPlacements/2016
        [Route("GetAllStudentsInYear/{year:int}")]
        public dynamic GetAllStudentsInYear(int year)
        {
            var StudentsforYear = (from s in db.Students
                                   join sps in db.StudentProgrammeStages
                                   on s.SID equals sps.SID
                                   where sps.Year == year
                                   select new
                                   {
                                       s.SID,
                                       s.FirstName,
                                       s.SecondName,
                                       sps.ProgrammeStageID
                                   });
            return StudentsforYear;
        }

        //GetStudentPlacementsInYear/SID/S0000090@mail.itsligo.ie/Year/2015
        [Route("GetStudentPlacementsInYear/SID/{sid:regex(^([a-zA-Z0-9_\\-\\.]+)@((\\[[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.)|(([a-zA-Z0-9\\-]+\\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\\]?)$)}/Year/{year:int}")]
        public dynamic GetStudentPlacementsInYear(string sid, int year)
        {
            return (from p in db.Placements
                    join sp in db.StudentPlacements
                    on p.PlacementID equals sp.PlacementID
                    join ss in db.StudentProgrammeStages
                    on sp.SID equals ss.SID
                    join s in db.Students
                    on ss.SID equals s.SID
                    where (s.SID == sid && ss.Year == year && p.StartDate.Value.Year == year)
                    select new
                    {
                        s.SID,
                        s.FirstName,
                        s.SecondName,
                        Location = new { p.AddressLine1, p.AddressLine2, p.City, p.County, p.Country },
                        p.StartDate,
                        p.FinishDate
                    });
        }


        // PUT: api/Students/5
        [ResponseType(typeof(void))]
        [Route("PutStudent/{id:int}")]
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
        [Route("PostStudent")]
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
        [Route("DeleteStudent/{id:int}")]
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