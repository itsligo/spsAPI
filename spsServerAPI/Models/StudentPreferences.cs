namespace spsServerAPI.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("StudentPreference")]
    public partial class StudentPreference
    {
        public StudentPreference()
        {
            //TutorVisits = new HashSet<TutorVisit>();
        }

        //[Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //public int SPID { get; set; }

        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [ForeignKey("placement")]
        public int PID { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [ForeignKey("student")]
        public string SID { get; set; }

        public int Preference { get; set; }

        public int? Status { get; set; }

        //public int? TutorID { get; set; }

        //public int? SupervisorID { get; set; }

        public DateTime TimeStamp { get; set; }

        public virtual Placement placement { get; set; }

        public virtual Student student { get; set; }

        //public virtual ICollection<TutorVisit> TutorVisits { get; set; }

        //public virtual Tutor Tutor { get; set; }
    }
}
