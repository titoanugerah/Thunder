using System.ComponentModel;

namespace Thunder.Models
{
    public class Audit
    {
        public int? CreatedById { set; get; }
        public User CreatedBy { set; get; }
        public DateTime? CreatedDate { set; get; }
        public int? UpdatedById { set; get; }  
        public User UpdatedBy { set; get; }
        public DateTime? UpdatedDate { set; get; }
        [DefaultValue(1)]
        public int IsExist { set; get; }
    }
}
