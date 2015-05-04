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


        [ForeignKey("student")]
        public string SID { get; set; }
        [ForeignKey("programmeStage")]
        public int ProgrammeStageID { get; set; }

        public int Year { get; set; }

        public virtual ProgrammeStage programmeStage { get; set; }

        public virtual Student student { get; set; }
    }
}
