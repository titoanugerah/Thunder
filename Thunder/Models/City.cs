using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Thunder.Models
{
    public class City : Audit
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public double EducationIndexScore { set; get; }
        public List<University> Universities { set; get; }
        [NotMapped]
        public double Total { set; get; }
        [NotMapped]
        public double Priority { set; get; }

    }
}
