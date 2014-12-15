namespace spsServerAPI.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ProgrammeListView")]
    public partial class ProgrammeListView
    {
        [Key]
        [StringLength(50)]
        public string ProgrammeCode { get; set; }

        public string ProgrammeName { get; set; }
    }
}
