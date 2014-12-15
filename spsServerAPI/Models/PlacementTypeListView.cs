namespace spsServerAPI.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PlacementTypeListView")]
    public partial class PlacementTypeListView
    {
        public int Id { get; set; }

        public string Description { get; set; }
    }
}
