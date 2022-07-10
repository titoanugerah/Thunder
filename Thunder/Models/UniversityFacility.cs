using System.ComponentModel.DataAnnotations;

namespace Thunder.Models
{
    public class UniversityFacility : Audit
    {
        [Key]
        public int Id { get; set; }
        public int UniversityId { set; get; }
        public University University { set; get; }
        public int FacilityId { set; get; }
        public Facility Facility { set; get; }
        public int Value { set; get; }
        public string Icon { set; get; }
    }
}
