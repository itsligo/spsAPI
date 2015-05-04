namespace spsServerAPI.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Placement")]
    public partial class Placement
    {
        public Placement()
        {
            allowablePlacements = new HashSet<AllowablePlacement>();
            studentPreferences = new HashSet<StudentPreference>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PlacementID { get; set; }
        [ForeignKey("placementProvider")]
        public int ProviderID { get; set; }
        [ForeignKey("assignedStudent")]
        public string AssignedStudentID { get; set; }
        [ForeignKey("placementSpervisor")]
        public int? SupervisorID { get; set; }
        [ForeignKey("placementTutor")]
        public int? AssignedTutorID { get; set; }

        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string PlacementDescription { get; set; }

        [ForeignKey("placementType")]
        public int PlacementTypeID { get; set; }
        //public string WebLink { get; set; }

        [Column(TypeName = "date")]
        public DateTime? StartDate { get; set; }
        [Column(TypeName = "date")]
        public DateTime? FinishDate { get; set; }
        public string County { get; set; }
        public string Country { get; set; }
        public bool Exclusive { get; set; }
        public bool? UseBaseAddress { get; set; }
        public virtual ICollection<AllowablePlacement> allowablePlacements { get; set; }
        public virtual PlacementProvider placementProvider { get; set; }
        public virtual PlacementType placementType { get; set; }
        public virtual Student assignedStudent { get; set; }
        public virtual ICollection<StudentPreference> studentPreferences { get; set; }
        public virtual PlacementSupervisor placementSpervisor { get; set; }
        public virtual Tutor placementTutor { get; set; }
    }
}
