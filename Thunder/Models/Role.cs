using System.ComponentModel.DataAnnotations;

namespace Thunder.Models
{
    public class Role : Audit
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
