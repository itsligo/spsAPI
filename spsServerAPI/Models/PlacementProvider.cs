namespace spsServerAPI.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PlacementProvider")]
    public partial class PlacementProvider
    {
        public PlacementProvider()
        {
            placements = new HashSet<Placement>();
            //PlacementSupervisors = new HashSet<PlacementSupervisor>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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
        
        public string WebLink { get; set; }

        // Supervisor Details

        //public string Supervisor { get; set; }

        //public string SupervisorSecondName { get; set; }

        //[StringLength(50)]
        //public string SupervisorContactNumber { get; set; }
        
        //[EmailAddress]
        //public string SupervisorEmail { get; set; }

        public virtual ICollection<Placement> placements { get; set; }

        //public virtual ICollection<PlacementSupervisor> PlacementSupervisors { get; set; }
    }
}
