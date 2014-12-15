namespace spsServerAPI.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("StudentPlacement")]
    public partial class StudentPlacement
    {
        public StudentPlacement()
        {
            TutorVisits = new HashSet<TutorVisit>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SPID { get; set; }

        [StringLength(50)]
        public string SID { get; set; }

        public int? PlacementID { get; set; }

        public int? Preference { get; set; }

        public int? Status { get; set; }

        public int? TutorID { get; set; }

        public int? SupervisorID { get; set; }

        public virtual Placement Placement { get; set; }

        public virtual Student Student { get; set; }

        public virtual ICollection<TutorVisit> TutorVisits { get; set; }

        public virtual Tutor Tutor { get; set; }
    }
}
