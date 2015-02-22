namespace spsServerAPI.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Placement")]
    public partial class Placement
    {
        public Placement()
        {
            AllowablePlacements = new HashSet<AllowablePlacement>();
            StudentPlacements = new HashSet<StudentPlacement>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PlacementID { get; set; }

        public int? ProviderID { get; set; }

        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        public string City { get; set; }

        public string PlacementDescription { get; set; }

        public int? PlacementType { get; set; }

        public string WebLink { get; set; }

        [Column(TypeName = "date")]
        public DateTime? StartDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? FinishDate { get; set; }

        public string County { get; set; }

        public string Country { get; set; }

        public bool? Filled { get; set; }

        public bool? UseBaseAddress { get; set; }

        public virtual ICollection<AllowablePlacement> AllowablePlacements { get; set; }

        public virtual PlacementProvider PlacementProvider { get; set; }

        public virtual PlacementType PlacementTypeRef { get; set; }

        //public virtual Placed PlacedPlacement { get; set; }

        public virtual ICollection<StudentPlacement> StudentPlacements { get; set; }
    }
}
