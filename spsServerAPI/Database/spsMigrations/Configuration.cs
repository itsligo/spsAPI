namespace spsServerAPI.Database.spsMigrations
{
    using Microsoft.AspNet.Identity.EntityFramework;
    using spsServerAPI.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Collections;
    using System.Collections.Generic;

    internal sealed class Configuration : DbMigrationsConfiguration<spsServerAPI.Models.Model>
    {
        
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"Database\spsMigrations";
        }

        //protected override void Seed(spsServerAPI.Models.Model context)
        //{
        //    if (System.Diagnostics.Debugger.IsAttached == false)
        //        System.Diagnostics.Debugger.Launch();

        //    //IdentityDbContext db = new IdentityDbContext();
            
        //    spsServerAPI.Models.ApplicationDbContext appUserDb = new Models.ApplicationDbContext();
        //    #region seed students
        //    // get all the users registered and see if there is a student record for them
        //    var userStore = new ApplicationUserStore(appUserDb);
        //    var roleStore = new ApplicationRoleStore(appUserDb);

        //    var userManager = new ApplicationUserManager(userStore);
        //    var roleManager = new ApplicationRoleManager(roleStore);

        //    var users = userManager.Users;
        //    var roles = roleManager.Roles;

        //    foreach (var item in users)
        //    {
        //        var userRole = item.Roles.First();
        //        var chosenRole = roles.Where(r => r.Id == userRole.RoleId 
        //                                        && r.Name == "unapproved");
        //        if (context.Students.Find(item.UserName) == null
        //            && chosenRole != null)

        //            context.Students.Add(new
        //            Student
        //            {
        //                SID = item.UserName,
        //                FirstName = item.FirstName,
        //                SecondName = item.SecondName
        //            });
        //    }
        //    context.SaveChanges();
        //    #endregion
        //    #region seed tutors
        //    foreach (var item in users)
        //    {
        //        var userRole = item.Roles.First();
        //        var chosenRole = roles.Where(r => r.Id == userRole.RoleId
        //                                        && r.Name == "tutors");
        //        if (context.Tutors.FirstOrDefault(t => t.Email == item.UserName) == null
        //            && chosenRole != null)
        //            context.Tutors.Add(new
        //            Tutor
        //            {
        //                Email = item.UserName,
        //                FirstName = item.FirstName,
        //                SecondName = item.SecondName,
        //                ContactNumber1 = "0719155222"
        //            });
        //    }
        //    context.SaveChanges();

        //    #endregion

        //    #region seed programmes
        //    List<Programme> progs = new List<Programme>();
            
        //        progs.Add(new Programme { ProgrammeCode = "SG_HCHILD_L8", ProgrammeName = "BA Hons in Early Childhood L8" });
        //        progs.Add(new Programme { ProgrammeCode = "SG_HEARL_H08", ProgrammeName = "BA Hons Early Childhood Care" });
        //        progs.Add(new Programme { ProgrammeCode = "SG_HSOCI_H08", ProgrammeName = "BA Hons Social Care Practice" });
        //        //progs.Add(new Programme { ProgrammeCode = "", ProgrammeName = "" });

        //        // Add Programmes
        //        foreach (var item in progs)
        //            if (context.Programmes.Find(item.ProgrammeCode) == null)
        //                context.Programmes.AddOrUpdate(item);
        //        context.SaveChanges();
            
        //    #endregion
        //    #region add programme stages and assign students
        //    // Add Programme Stages
        //    if (context.Programmes.Count() > 0)
        //    {
        //        for (int i = 2; i < 5; i++)
        //        {
        //            foreach (var item in context.Programmes)
        //                if(context.ProgrammeStages.Where(ps => ps.ProgrammeCode == item.ProgrammeCode 
        //                    && ps.Stage == i).Count() == 0)
        //                        context.ProgrammeStages.Add(new
        //                                ProgrammeStage { ProgrammeCode = item.ProgrammeCode, Stage = i });
        //        }
        //        context.SaveChanges();
        //    }
        //    if (context.StudentProgrammeStages.Count()  == 0)
        //    {
        //        Random r = new Random();
        //        int psCount = context.ProgrammeStages.Max(ps => ps.Id);
        //        // Assign students to programme stages
        //        foreach (var item in context.Students)
        //        {
        //            if (context.StudentProgrammeStages.FirstOrDefault(s => s.SID == item.SID) == null)
        //            {
        //                context.StudentProgrammeStages.AddOrUpdate(
        //                    new
        //                    StudentProgrammeStage
        //                    {
        //                        ProgrammeStageID = r.Next(1, psCount),
        //                        SID = item.SID
        //                    });
        //            }
        //        }
        //        context.SaveChanges();
        //    }
        //    #endregion
        //    #region placement types
        //    if (context.PlacementTypes == null)
        //    {
        //        context.PlacementTypes.Add(new PlacementType { Description = "Placement type 1" });
        //        context.PlacementTypes.Add(new PlacementType { Description = "Placement type 2" });
        //        context.PlacementTypes.Add(new PlacementType { Description = "Placement type 3" });
        //    }
        //    context.SaveChanges();
        //    #endregion
        //    #region seed placement providers
        //    string[] cities = { "Dublin", "Cork", "Limerick", "Galway", "Sligo" };
        //    string[] county = { "Dublin", "Cork", "Limerick", "Galway", "Sligo" };

        //    // create placement providers
        //    if (context.PlacementProviders == null)
        //    {
        //        Random r = new Random();
        //        List<PlacementProvider> providers = new List<PlacementProvider>();
        //        for (int i = 2; i < 5; i++)
        //        {

        //            int chosen = r.Next(0, cities.Length - 1);
        //            providers.Add(
        //                new PlacementProvider
        //                {
        //                    ProviderName = "Provider" + cities[chosen],
        //                    AddressLine1 = "Address line " + cities[chosen],
        //                    AddressLine2 = "Address line" + cities[chosen],
        //                    City = cities[chosen],
        //                    County = county[chosen],
        //                    ProviderDescription = "Provider description in " + county[chosen]

        //                }
        //                );
        //        }
        //        context.SaveChanges();
        //        foreach (PlacementProvider p in context.PlacementProviders)
        //        {
        //            if (p.Placements == null)
        //            {
        //                int max = 1;
        //                if(context.PlacementTypes.Count() > 0)
        //                    max = context.PlacementTypes.Max().Id;
        //                // heads or tails for placement addressses
        //                int placementType = r.Next(1, max);    
        //                if (r.Next(0, 1) == 0)
        //                {
        //                    p.Placements.Add(new Placement
        //                    {
        //                        AddressLine1 = "Overridden Address Line 1",
        //                        AddressLine2 = "Overridden Address Line 2",
        //                        City = "Overridden City",
        //                        Country = "Overridden County",
        //                        UseBaseAddress = false,
        //                        PlacementDescription = p.ProviderName + "Placement Description",
        //                        Filled = false,
        //                        ProviderID = p.ProviderID,
        //                        StartDate = DateTime.Now.AddMonths(r.Next(2, 4)),
        //                        FinishDate = DateTime.Now.AddMonths(r.Next(5, 9)),
        //                        //PlacementProvider = p,
        //                        PlacementType = placementType
        //                    });
        //                }
        //                else // inherit address
        //                {
        //                    p.Placements.Add(new Placement
        //                    {
        //                        UseBaseAddress = true,
        //                        PlacementDescription = p.ProviderName + "Placement Description",
        //                        Filled = false,
        //                        ProviderID = p.ProviderID,
        //                        StartDate = DateTime.Now.AddMonths(r.Next(2, 4)),
        //                        FinishDate = DateTime.Now.AddMonths(r.Next(5, 9)),
        //                        PlacementType = placementType
        //                        //PlacementProvider = p
        //                    });

        //                }
        //            }
        //        }
        //    }
        //    #endregion
            
        //    #region allowable placements
        //    if (context.AllowablePlacements == null)
        //    {
        //        Random r = new Random();
        //        // one allowable placement per placement with random stage assignment
        //        foreach (Placement p in context.Placements)
        //        {
        //            p.AllowablePlacements = new AllowablePlacement[] 
        //                { new AllowablePlacement {
        //                        PlacementID = r.Next(1,context.Placements.Count()),
        //                          ProgrammeStageID = r.Next(1,context.ProgrammeStages.Count())
        //                         }
        //            };
        //        }
        //    }
        //    #endregion

        //    #region student placements for allowable placements
        //    // if there are students then for allowable placements for this student
        //    // allow them to apply for a student placement
        //    if (context.Students != null)
        //    {
        //        foreach (Student s in context.Students)
        //        {
        //            var allowableplacements =
        //                (from ap in context.AllowablePlacements
        //                 join ps in context.ProgrammeStages
        //                  on ap.ProgrammeStageID equals ps.Id
        //                 join ss in context.StudentProgrammeStages
        //                     on ps.Id equals ss.ProgrammeStageID
        //                 where ss.SID == s.SID
        //                 select ap);
        //            if (allowableplacements.Count() > 0)
        //            {
        //                Random r = new Random();
        //                int preference = 0;
        //                foreach (AllowablePlacement ap in allowableplacements)
        //                {
        //                    // only allowing upto 3 preferenced placements
        //                   if(++preference < 4)
        //                   {
        //                       s.StudentPlacements.Add(new StudentPlacement
        //                       {
        //                           PlacementID = ap.PlacementID,
        //                           SID = s.SID,
        //                           Preference = preference,
        //                           Status = 0 // unfilled 1 is filled
        //                       });
        //                   }

        //                }
        //            }

        //        }
        //    }
        //    #endregion
        //} // end seed
    }
}