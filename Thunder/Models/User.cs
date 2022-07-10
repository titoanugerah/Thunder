using System.ComponentModel.DataAnnotations;

namespace Thunder.Models
{
    public class User : Audit
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string RoleId { get; set; }
        public Role Role { get; set; }
    }
}
