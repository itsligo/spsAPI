namespace spsServerAPI.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PlacementProviderView")]
    public partial class PlacementProviderView
    {
        [Key]
        public int ProviderID { get; set; }

        public string ProviderName { get; set; }

        public string ContactNumber { get; set; }

        public string ProviderDescription { get; set; }

        public string AdditionalNotes { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string City { get; set; }

        public string County { get; set; }

        public string Country { get; set; }
    }
}
