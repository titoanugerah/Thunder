using System.ComponentModel.DataAnnotations;

namespace Thunder.Models
{
    public class Province : Audit
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public List<City> Cities { set; get; }
    }
}
