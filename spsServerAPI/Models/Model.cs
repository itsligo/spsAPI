namespace spsServerAPI.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Model : DbContext
    {
        public Model()
            : base("name=spsModel")
        {
            this.Configuration.LazyLoadingEnabled = false;
        }
        public virtual DbSet<AllowablePlacement> AllowablePlacements { get; set; }
        public virtual DbSet<Placement> Placements { get; set; }
        public virtual DbSet<PlacementProvider> PlacementProviders { get; set; }
        public virtual DbSet<PlacementSupervisor> PlacementSupervisors { get; set; }
        public virtual DbSet<PlacementType> PlacementTypes { get; set; }
        public virtual DbSet<Programme> Programmes { get; set; }
        public virtual DbSet<ProgrammeStage> ProgrammeStages { get; set; }
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<StudentPlacement> StudentPlacements { get; set; }
        public virtual DbSet<StudentProgrammeStage> StudentProgrammeStages { get; set; }
        public virtual DbSet<Tutor> Tutors { get; set; }
        public virtual DbSet<TutorVisit> TutorVisits { get; set; }
        //// The following are to be dropped from the Database model
        //public virtual DbSet<AvailablePlacementsView> AvailablePlacementsViews { get; set; }
        //public virtual DbSet<PlacementProviderView> PlacementProviderViews { get; set; }
        //public virtual DbSet<PlacementTypeListView> PlacementTypeListViews { get; set; }
        //public virtual DbSet<PlacementView> PlacementViews { get; set; }
        //public virtual DbSet<PlacementYearsView> PlacementYearsViews { get; set; }
        //public virtual DbSet<ProgrammeListView> ProgrammeListViews { get; set; }
        //public virtual DbSet<ProgrammeStagesView> ProgrammeStagesViews { get; set; }
        //public virtual DbSet<ProvidersNameListView> ProvidersNameListViews { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Placement>()
                .HasMany(e => e.StudentPlacements)
                .WithOptional(e => e.Placement)
                .WillCascadeOnDelete();

            modelBuilder.Entity<PlacementProvider>()
                .HasMany(e => e.Placements)
                .WithOptional(e => e.PlacementProvider)
                .WillCascadeOnDelete();

            modelBuilder.Entity<PlacementType>()
                .HasMany(e => e.Placements)
                .WithOptional(e => e.PlacementTypeRef)
                .HasForeignKey(e => e.PlacementType);

            modelBuilder.Entity<Programme>()
                .HasMany(e => e.ProgrammeStages)
                .WithOptional(e => e.Programme)
                .WillCascadeOnDelete();

            modelBuilder.Entity<ProgrammeStage>()
                .HasMany(e => e.StudentProgrammeStages)
                .WithOptional(e => e.ProgrammeStage)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Student>()
                .HasMany(e => e.StudentProgrammeStages)
                .WithOptional(e => e.Student)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Student>()
                .HasMany(e => e.StudentPlacements)
                .WithOptional(e => e.Student)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Tutor>()
                .HasMany(e => e.StudentPlacements)
                .WithOptional(e => e.Tutor)
                .WillCascadeOnDelete();

            modelBuilder.Entity<Tutor>()
                .HasMany(e => e.TutorVisits)
                .WithOptional(e => e.Tutor)
                .WillCascadeOnDelete();
        }
    }
}
