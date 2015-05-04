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
            //StudentPlacements = new HashSet<Placed>();
            tutorVisits = new HashSet<TutorVisit>();
        }
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TutorID { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string SecondName { get; set; }

        [StringLength(50)]
        [Phone]
        public string ContactNumber1 { get; set; }

        [StringLength(50)]
        [Phone]
        public string ContactNumber2 { get; set; }

        //public virtual ICollection<Placement> studentPlacements { get; set; }

        public virtual ICollection<TutorVisit> tutorVisits { get; set; }
    }
}
