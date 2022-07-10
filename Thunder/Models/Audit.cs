using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Thunder.Models
{
    public class Audit
    {
        public int? CreatedById { set; get; }
        //[ForeignKey("CreatedById")]
        //public User CreatedBy { set; get; }
        public DateTime? CreatedDate { set; get; }
        //public int? UpdatedById { set; get; }
        //[ForeignKey("UpdatedById")]
        //public User UpdatedBy { set; get; }
        public DateTime? UpdatedDate { set; get; }
        [DefaultValue(1)]
        public int IsExist { set; get; }
    }
}
