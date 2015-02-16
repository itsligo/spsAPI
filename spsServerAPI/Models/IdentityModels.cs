using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System.Data.Entity;
using System;
using System.ComponentModel.DataAnnotations;
using spsServerAPI.Database.UserManagementMigrations;

namespace spsServerAPI.Models
{

    // You will not likely need to customize there, but it is necessary/easier to create our own 
    // project-specific implementations, so here they are:
    public class ApplicationUserLogin : IdentityUserLogin<string> { }
    public class ApplicationUserClaim : IdentityUserClaim<string> { }
    public class ApplicationUserRole : IdentityUserRole<string> { }

    // Must be expressed in terms of our custom Role and other types:
    public class ApplicationUser
        : IdentityUser<string, ApplicationUserLogin,
        ApplicationUserRole, ApplicationUserClaim>
    {
        public ApplicationUser()
        {
            this.Id = Guid.NewGuid().ToString();
        }


        // ** Add authenticationtype as method parameter:
        public async Task<ClaimsIdentity>
            GenerateUserIdentityAsync(ApplicationUserManager manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
        public int ProgrammeStageID { get; set; }
        public bool Approved { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }

        //public string Address { get; set; }
        //public string City { get; set; }
        //public string State { get; set; }
        //public string PostalCode { get; set; }
    }


    // Must be expressed in terms of our custom UserRole:
    public class ApplicationRole : IdentityRole<string, ApplicationUserRole>
    //public class ApplicationRole : IdentityRole
    {
        public ApplicationRole()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public ApplicationRole(string name)
            : this()
        {
            this.Name = name;
        }


        public string Description { get; set; }
    }


    // Must be expressed in terms of our custom types:
    public class ApplicationDbContext 
        //: IdentityDbContext<ApplicationUser>
        : IdentityDbContext<ApplicationUser, ApplicationRole,
         string, ApplicationUserLogin, ApplicationUserRole, ApplicationUserClaim>
    {
         public ApplicationDbContext()
            : base("spsModel")
        {
        }
        public DbSet<Client> Clients { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        //public DbSet<ApplicationRole> ApplicationRoles { get; set; }
        //public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        //public DbSet<ApplicationUserRole> ApplicationUserRoles { get; set; }

        //static ApplicationDbContext()
        //{
        //    //System.Data.Entity.Database.SetInitializer<ApplicationDbContext>(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Configuration>());
        //    return this;
        //}

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        // Add additional items here as needed
        
    }

    // Most likely won't need to customize these either, but they were needed because we implemented
    // custom versions of all the other types:
    public class ApplicationUserStore 
        ///: UserStore<ApplicationUser>
        : UserStore<ApplicationUser, ApplicationRole, string,
            ApplicationUserLogin, ApplicationUserRole,
            ApplicationUserClaim>, IUserStore<ApplicationUser, string>,
        IDisposable
    {
        public ApplicationUserStore()
            : this(new IdentityDbContext())
        {
            //base.DisposeContext = true;
        }

        public ApplicationUserStore(DbContext context)
            : base(context)
        {
        }
    }


    public class ApplicationRoleStore
        //: RoleStore<ApplicationRole>
    : RoleStore<ApplicationRole, string, ApplicationUserRole>,
    IQueryableRoleStore<ApplicationRole, string>,
    IRoleStore<ApplicationRole, string>, IDisposable
    {
        public ApplicationRoleStore()
            : base(new IdentityDbContext())
        {
            //base.DisposeContext = true;
        }

        public ApplicationRoleStore(DbContext context)
            : base(context)
        {
        }
    }
}