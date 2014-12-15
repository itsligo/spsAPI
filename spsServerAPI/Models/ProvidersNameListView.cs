namespace spsServerAPI.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ProvidersNameListView")]
    public partial class ProvidersNameListView
    {
        [Key]
        public int ProviderID { get; set; }

        public string ProviderName { get; set; }
    }
}
