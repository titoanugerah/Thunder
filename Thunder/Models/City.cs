using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Thunder.Models
{
    public class City : Audit
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int ProvinceId { set; get; }
        [ForeignKey("ProvinceId")]
        public Province Province { set; get; }
    }
}
