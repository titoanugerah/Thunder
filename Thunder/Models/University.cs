using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Thunder.Models
{
    public class University : Audit
    {
        [Key]
        public int Id { get; set; } 
        [Required]
        public string Name { get; set; }
 
        public Accreditation Accreditation { set; get; }
        public string Description { get; set; }
        [DefaultValue(0)]
        public int TuitionFee { set; get; }
        public string Address { set; get; }
        [Required]
        public int CityId { set; get; }
        [ForeignKey("CityId")]
        public City City { set; get; }
        public string MapsUrl { set; get; }
        public List<UniversityFacility> UniversityFacilities { set; get; }
        [DefaultValue("")]
        public string? Logo { set; get; }
        //[MaxLength(1)]
        //public string Accreditation { get;set; }
        public int AccreditationId { get; set; }

        [NotMapped]
        public double ScoreAccreditation { set; get; }

        [NotMapped]
        public double ScoreCity { set; get; }
        [NotMapped]
        public double ScoreFacility { set; get; }

    }
}
