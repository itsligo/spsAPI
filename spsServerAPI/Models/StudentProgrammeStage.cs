namespace spsServerAPI.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("StudentProgrammeStage")]
    public partial class StudentProgrammeStage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MemberID { get; set; }

        [StringLength(50)]
        public string SID { get; set; }

        public int? ProgrammeStageID { get; set; }

        public int? Year { get; set; }

        public virtual ProgrammeStage ProgrammeStage { get; set; }

        public virtual Student Student { get; set; }
    }
}
