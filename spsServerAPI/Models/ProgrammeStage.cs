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
            AllowablePlacements = new HashSet<AllowablePlacement>();
            StudentProgrammeStages = new HashSet<StudentProgrammeStage>();
        }
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(50)]
        public string ProgrammeCode { get; set; }

        public int? Stage { get; set; }

        public virtual ICollection<AllowablePlacement> AllowablePlacements { get; set; }

        public virtual Programme Programme { get; set; }

        public virtual ICollection<StudentProgrammeStage> StudentProgrammeStages { get; set; }
    }
}
