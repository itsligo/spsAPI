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
            AutomaticMigrationsEnabled = true;
            MigrationsDirectory = @"Database\spsMigrations";
        }

        protected override void Seed(spsServerAPI.Models.Model context)
        {
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
            spsServerAPI.Models.ApplicationDbContext appUserDb = new Models.ApplicationDbContext();

            // get all the users registered and see if there is a student reord for them
            var userStore = new ApplicationUserStore(appUserDb);
            var userManager = new ApplicationUserManager(userStore); 
            var users = userManager.Users;
            foreach (var item in users)
            {
                if (context.Students.Find(item.UserName) == null)
                    context.Students.Add(new
                    Student
                    {
                         SID = item.UserName,
                         FirstName = item.FirstName,
                         SecondName = item.SecondName
                    });
            }
            context.SaveChanges();

            List<Programme> progs = new List<Programme>();
            progs.Add(new Programme { ProgrammeCode = "SG_HCHILD_L8", ProgrammeName = "BA Hons in Early Childhood L8" });
            progs.Add(new Programme { ProgrammeCode = "SG_HEARL_H08", ProgrammeName = "BA Hons Early Childhood Care" });
            progs.Add(new Programme { ProgrammeCode = "SG_HSOCI_H08", ProgrammeName = "BA Hons Social Care Practice" });
            //progs.Add(new Programme { ProgrammeCode = "", ProgrammeName = "" });

            // Add Programmes
            foreach (var item in progs)
                context.Programmes.AddOrUpdate(item);
            context.SaveChanges();

            // Add Programme Stages
            for (int i = 2; i < 5; i++)
            {
                foreach (var item in progs)
                    context.ProgrammeStages.AddOrUpdate(new ProgrammeStage { ProgrammeCode = item.ProgrammeCode, Stage = i });
            }
            context.SaveChanges();
            Random r = new Random();
            int psCount = context.ProgrammeStages.Max(ps => ps.Id);
            // Assign students to programme stages
            foreach (var item in context.Students)
            {
                if(context.StudentProgrammeStages.FirstOrDefault(s => s.SID == item.SID) == null)
                {
                    context.StudentProgrammeStages.AddOrUpdate(
                        new 
                        StudentProgrammeStage 
                        { ProgrammeStageID = r.Next(1,psCount),
                          SID = item.SID 
                        });
                }
            }
            string[] cities = {"Dublin","Cork","Limerick","Galway","Sligo"};
            string[] county = { "Dublin", "Cork", "Limerick", "Galway", "Sligo" };

            // create placement providers
            List<PlacementProvider> providers = new List<PlacementProvider>();
            int chosen = r.Next(0, cities.Length-1);
            providers.Add(
                new PlacementProvider
                {
                    ProviderName = "Provider",
                    AddressLine1 = "Address line ",
                    AddressLine2 = "Address line",
                    City = cities[chosen],
                    County = county[chosen],
                    ProviderDescription = "Provider description"
                }
                );
        }
    }
}
