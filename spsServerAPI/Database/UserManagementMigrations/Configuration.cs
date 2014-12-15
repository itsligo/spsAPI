namespace spsServerAPI.Database.UserManagementMigrations
{
    using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using spsServerAPI.Models;
using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;

    internal sealed class Configuration : DbMigrationsConfiguration<spsServerAPI.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            MigrationsDirectory = @"Database\UserManagementMigrations";
        }

        protected override void Seed(spsServerAPI.Models.ApplicationDbContext context)
        {
            spsServerAPI.Models.Model spsdb = new Model();

            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //

            #region setup Roles
            const string adminName = "spsAdmin@itsligo.ie";
            const string password = "Admin$1";
            string[] roleNames = new string[4] { "admin", "student", "tutor", "unapproved" };
            var roleStore = new RoleStore<IdentityRole>(context);
                var roleManager = new RoleManager<IdentityRole>(roleStore);
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
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new ApplicationUserManager(userStore);

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
        } // End Seed


    }
}
