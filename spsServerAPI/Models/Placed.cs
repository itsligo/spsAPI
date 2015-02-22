

namespace spsServerAPI.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    
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
        public int Year { get; set; }
        public Nullable<int> PlacedPlacement_PlacementID { get; set; }
    
        public virtual Placement Placement { get; set; }
        public virtual Student Student { get; set; }
    }
}
