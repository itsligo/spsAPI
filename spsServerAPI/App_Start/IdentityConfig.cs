using spsServerAPI.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System.Data.Entity;
using System.Web;
using System;
using System.Collections.Generic;
using spsServerAPI.Providers;

namespace spsServerAPI
{
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.


    public class ApplicationUserManager
        : UserManager<ApplicationUser, string>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser, string> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(
            IdentityFactoryOptions<ApplicationUserManager> options,
            IOwinContext context)
        {
            var manager = new ApplicationUserManager(
                new UserStore<ApplicationUser, ApplicationRole, string,
                    ApplicationUserLogin, ApplicationUserRole,
                    ApplicationUserClaim>(context.Get<ApplicationDbContext>()));

            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
            };
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                    new DataProtectorTokenProvider<ApplicationUser>(
                        dataProtectionProvider.Create("ASP.NET Identity"));
            }
            return manager;
        }
    }


    public class ApplicationRoleManager : RoleManager<ApplicationRole>
    {
        public ApplicationRoleManager(IRoleStore<ApplicationRole, string> roleStore)
            : base(roleStore)
        {
        }

        public static ApplicationRoleManager Create(
            IdentityFactoryOptions<ApplicationRoleManager> options,
            IOwinContext context)
        {
            return new ApplicationRoleManager(
                new ApplicationRoleStore(context.Get<ApplicationDbContext>()));
        }
    }


    // This is useful if you do not want to tear down the database each time you run the application.
    // public class ApplicationDbInitializer : DropCreateDatabaseAlways<ApplicationDbContext>
    // This example shows you how to create a new database if the Model changes
    public class ApplicationDbInitializer
        : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            //InitializeIdentityForEF(context);
            base.Seed(context);
        }
        public static void InitializeIdentityForEF(ApplicationDbContext db)
        {
            var userManager = HttpContext.Current
                .GetOwinContext().GetUserManager<ApplicationUserManager>();

            var roleManager = HttpContext.Current
                .GetOwinContext().Get<ApplicationRoleManager>();
            spsServerAPI.Models.Model spsdb = new Model();

            #region setup Roles
            const string adminName = "spsAdmin@itsligo.ie";
            const string password = "Admin$1";
            string[] roleNames = new string[4] { "admin", "student", "tutor", "unapproved" };
            //var roleStore = new RoleStore<IdentityRole>(context);
            //var roleManager = new RoleManager<IdentityRole>(roleStore);
            foreach (string roleName in roleNames)
            {
                var role = roleManager.FindByName(roleName);
                if (role == null)
                {
                    role = new ApplicationRole(roleName);
                    var roleresult = roleManager.Create(role);
                }
            }
            #endregion

            #region Create Admin Account
            // Create admin user
            //var userStore = new UserStore<ApplicationUser>(context);
            //var userManager = new ApplicationUserManager(userStore);

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

            // pp jan 2015
            if (db.Clients.CountAsync().Result > 0)
            {
                return;
            }

            db.Clients.AddRange(BuildClientsList());
            db.SaveChanges();

        }
        private static List<Client> BuildClientsList()
        {

            List<Client> ClientsList = new List<Client> 
            {
                new Client
                { Id = "ngAuthApp", 
                    Secret= Helper.GetHash("abc@123"), 
                    Name="AngularJS front-end Application", 
                    ApplicationType =  Models.ApplicationTypes.JavaScript, 
                    Active = true, 
                    RefreshTokenLifeTime = 7200, 
                    AllowedOrigin = "http://localhost:53848/"
                },
                new Client
                { Id = "consoleApp", 
                    Secret=Helper.GetHash("123@abc"), 
                    Name="Console Application", 
                    ApplicationType =Models.ApplicationTypes.NativeConfidential, 
                    Active = true, 
                    RefreshTokenLifeTime = 14400, 
                    AllowedOrigin = "*"
                }
            };

            return ClientsList;
        }
    }
}
