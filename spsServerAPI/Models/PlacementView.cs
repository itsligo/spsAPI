namespace spsServerAPI.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PlacementView")]
    public partial class PlacementView
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int PlacementID { get; set; }

        public int? ProviderID { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string City { get; set; }

        public string PlacementDescription { get; set; }

        public int? PlacementType { get; set; }

        public string WebLink { get; set; }

        [Column(TypeName = "date")]
        public DateTime? StartDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? FinishDate { get; set; }

        public string County { get; set; }

        public string Country { get; set; }

        public bool? Filled { get; set; }

        public bool? UseBaseAddress { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [StringLength(50)]
        public string ProgrammeCode { get; set; }

        public int? Stage { get; set; }

        public string ProgrammeName { get; set; }
    }
}
