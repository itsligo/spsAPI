namespace spsServerAPI.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Programme")]
    public partial class Programme
    {
        public Programme()
        {
            programmeStages = new HashSet<ProgrammeStage>();
        }

        [Key]
        [StringLength(50)]
        public string ProgrammeCode { get; set; }

        public string ProgrammeName { get; set; }

        public virtual ICollection<ProgrammeStage> programmeStages { get; set; }
    }
}
