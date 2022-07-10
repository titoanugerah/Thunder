using System.ComponentModel.DataAnnotations;

namespace Thunder.Models
{
    public class Facility
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }         
    }
}
