namespace spsServerAPI.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ProgrammeStage")]
    public partial class ProgrammeStage
    {
        public ProgrammeStage()
        {
            allowablePlacements = new HashSet<AllowablePlacement>();
            studentProgrammeStages = new HashSet<StudentProgrammeStage>();
        }
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        [ForeignKey("programme")]
        public string ProgrammeCode { get; set; }

        public int? Stage { get; set; }

        public virtual ICollection<AllowablePlacement> allowablePlacements { get; set; }

        public virtual Programme programme { get; set; }

        public virtual ICollection<StudentProgrammeStage> studentProgrammeStages { get; set; }
    }
}
