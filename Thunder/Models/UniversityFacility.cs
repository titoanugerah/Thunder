using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Thunder.Models
{
    public class UniversityFacility : Audit
    {
        [Key]
        public int Id { get; set; }
        public int UniversityId { set; get; }
        [ForeignKey("UniversityId")]
        public University University { set; get; }
        public int FacilityId { set; get; }
        [ForeignKey("FacilityId")]
        public Facility Facility { set; get; }
        public int Value { set; get; }
    }
}
