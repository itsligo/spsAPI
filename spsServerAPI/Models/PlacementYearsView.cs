namespace spsServerAPI.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PlacementYearsView")]
    public partial class PlacementYearsView
    {
        [Key]
        public int PlacementID { get; set; }

        public int? PlacementYear { get; set; }
    }
}
