namespace spsServerAPI.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Tutor")]
    public partial class Tutor
    {
        public Tutor()
        {
            StudentPlacements = new HashSet<StudentPlacement>();
            TutorVisits = new HashSet<TutorVisit>();
        }
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TutorID { get; set; }

        public string FirstName { get; set; }

        public string SecondName { get; set; }

        [StringLength(50)]
        public string ContactNumber1 { get; set; }

        [StringLength(50)]
        public string ContactNumber2 { get; set; }

        public virtual ICollection<StudentPlacement> StudentPlacements { get; set; }

        public virtual ICollection<TutorVisit> TutorVisits { get; set; }
    }
}
