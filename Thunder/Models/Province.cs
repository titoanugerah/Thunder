using System.ComponentModel.DataAnnotations;

namespace Thunder.Models
{
    public class Province
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public List<City> Cities { set; get; }
    }
}
