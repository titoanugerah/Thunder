using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Thunder.Models
{
    public class Accreditation
    {
        [Required, Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Score { set; get; }
        [NotMapped]
        public double Total { set; get; }
    }
}
