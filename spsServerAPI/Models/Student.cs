namespace spsServerAPI.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Student")]
    public partial class Student
    {
        public Student()
        {
            StudentProgrammeStages = new HashSet<StudentProgrammeStage>();
            StudentPreferences = new HashSet<StudentPreference>();

        }

        [Key]
        [StringLength(50)]
        public string SID { get; set; }

        public string FirstName { get; set; }

        public string SecondName { get; set; }

        public virtual ICollection<StudentProgrammeStage> StudentProgrammeStages { get; set; }

        //public virtual Placed StudentPlaced { get; set; }

        public virtual ICollection<StudentPreference> StudentPreferences { get; set; }

        //public virtual Placement assignedPlacement { get; set; }
    }
}
