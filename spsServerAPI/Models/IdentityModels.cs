using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;
using System.Web;
using System;

namespace spsServerAPI.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public int ProgrammeStageID { get; set; }
        public bool Approved { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }


        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("spsModel", throwIfV1Schema: false)
        {
         
        }

        //static ApplicationDbContext()
        //{
        //    // Set the database intializer which is run once during application start
        //    // This seeds the database with admin user credentials and admin role
        //    //Database.SetInitializer<ApplicationDbContext>(new ApplicationDbInitializer());
        //}

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
    }
    public class ApplicationDbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {

        protected override void Seed(ApplicationDbContext context)
        {
            InitializeIdentityForEF(context);
            base.Seed(context);
        }

        //Create Users and roles
        public static void InitializeIdentityForEF(ApplicationDbContext db)
        {
            // load in entities to retrieve spsEntities for programme stage assignment
            Model spsdb = new Model();

            var userManager =
                HttpContext.Current.GetOwinContext()
                    .GetUserManager<ApplicationUserManager>();

            var roleManager =
                HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();

            const string adminName = "powell.paul@itsligo.ie";
            const string password = "Admin$1";
            string[] roleNames = new string[4] { "admin", "student", "tutor", "unapproved" };

            #region Create Roles
            //Create Roles if they do not exist       
            foreach (string roleName in roleNames)
            {
                var role = roleManager.FindByName(roleName);
                if (role == null)
                {
                    role = new IdentityRole(roleName);
                    var roleresult = roleManager.Create(role);
                }
            }
            #endregion
            #region Create Admin Account
            // Create admin user
            var user = userManager.FindByName(adminName);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = adminName,
                    FirstName = "Paul",
                    SecondName = "Powell",
                    Approved = true,
                    ProgrammeStageID = 0,
                    Email = adminName
                };
                var result = userManager.Create(user, password);
                result = userManager.SetLockoutEnabled(user.Id, false);
            }

            // Add user admin to Role Admin if not already added         
            var rolesForUser = userManager.GetRoles(user.Id);
            if (!rolesForUser.Contains(roleNames[0]))
            {
                var result = userManager.AddToRole(user.Id, roleNames[0]);
            }
            #endregion

            #region Create student test accounts
            PasswordHasher p = new PasswordHasher();
            //string adminPass = p.HashPassword("Admin$1");
            // Create 10 test users:
            Random r = new Random();
            int count = 0;
            spsdb.ProgrammeStages.CountAsync().ContinueWith((cont) => { count = cont.Result; });
            for (int i = 0; i < 10; i++)
            {
                var student = new ApplicationUser()
                {

                    UserName = string.Format("S000009{0}@mail.itsligo.ie", i.ToString()),
                    PasswordHash = p.HashPassword("S000009" + i.ToString()),
                    Email = string.Format("S000009{0}@mail.itsligo.ie", i.ToString()),
                    Approved = false,
                    FirstName = "Student First name " + i.ToString(),
                    SecondName = "Student Second name " + i.ToString(),
                    ProgrammeStageID = r.Next(1, count),
                    LockoutEnabled = false
                };

                //spsdb.Students.Add(new Student { SID = string.Format("S000009{0}", i.ToString())  });

                IdentityResult studentResult = userManager.Create(student);
                if (studentResult.Succeeded)
                {
                    var rolesForStudent = userManager.GetRoles(student.Id);
                    if (rolesForStudent == null || !rolesForStudent.Contains(roleNames[3]))
                    {
                        var result = userManager.AddToRole(student.Id, roleNames[3]);
                    }
                }

            }
            #endregion
            #region Create Tutors
            // Create Tutors
            for (int i = 0; i < 4; i++)
            {
                var tutor = new ApplicationUser()
                {
                    UserName = string.Format("tutor{0}@mail.itsligo.ie", i.ToString()),
                    FirstName = "Tutor First name " + i.ToString(),
                    SecondName = "Tutor Second name " + i.ToString(),
                    PasswordHash = p.HashPassword("Tutor$" + i.ToString()),
                    Email = string.Format("tutor{0}@mail.itsligo.ie", i.ToString()),
                    Approved = true,
                    LockoutEnabled = false
                };
                userManager.Create(tutor);


                //manager.Create(user, string.Format("S000009{0}", i.ToString()));

                var rolesForTutor = userManager.GetRoles(tutor.Id);
                if (rolesForTutor == null || !rolesForTutor.Contains(roleNames[2]))
                {
                    var result = userManager.AddToRole(tutor.Id, roleNames[2]);
                }

            }
            #endregion
        }
    }


}