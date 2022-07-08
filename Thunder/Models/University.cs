using System.ComponentModel.DataAnnotations;

namespace Thunder.Models
{
    public class University
    {
        [Key]
        public int Id { get; set; } 
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; } 
        public string Address { set; get; }
        [Required]
        public int CityId { set; get; }
        public City City { set; get; }

    }
}
