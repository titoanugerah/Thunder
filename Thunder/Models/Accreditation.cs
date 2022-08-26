using System.ComponentModel.DataAnnotations;

namespace Thunder.Models
{
    public class Accreditation
    {
        [Required, Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Score { set; get; }
    }
}
