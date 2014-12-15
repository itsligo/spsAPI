namespace spsServerAPI.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PlacementSupervisor")]
    public partial class PlacementSupervisor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? ProviderID { get; set; }

        public string FirstName { get; set; }

        public string SecondName { get; set; }

        [StringLength(50)]
        public string Telephone { get; set; }

        public string email { get; set; }

        public virtual PlacementProvider PlacementProvider { get; set; }
    }
}
