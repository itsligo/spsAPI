namespace spsServerAPI.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ProgrammeStagesView")]
    public partial class ProgrammeStagesView
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [StringLength(50)]
        public string ProgrammeCode { get; set; }

        public string ProgrammeName { get; set; }

        public int? Stage { get; set; }
    }
}
