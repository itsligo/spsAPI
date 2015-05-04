namespace spsServerAPI.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TutorVisit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VisitID { get; set; }

        public int TutorID { get; set; }
        [ForeignKey("assignedPlacement")]
        public int PlacementID { get; set; }

        [Column(TypeName = "date")]
        public DateTime DateVisited { get; set; }

        public string Comment { get; set; }

        public virtual Placement assignedPlacement { get; set; }
        //public virtual Placed placed { get; set; }

        public virtual Tutor tutor { get; set; }
    }
}
