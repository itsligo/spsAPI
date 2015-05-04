

namespace spsServerAPI.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    
    [Table("Placed")]
    public partial class Placed
    {

        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PID { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string SID { get; set; }

        //[Key]
        //public int PlacedId { get; set; }

        //[ForeignKey("studentPreference")]
        //public int StudentPrefernceID { get; set; }

        
        //public int? TutorID { get; set; }

        //public int Year { get; set; }

        //public virtual Placement PlacedPlacement { get; set; }
        //public virtual StudentPreference studentPreference { get; set; }

        //public virtual Tutor assignedTutor { get; set; }
        //public virtual ICollection<TutorVisit> TutorVisits { get; set; }

        
    }
}
