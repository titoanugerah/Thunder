using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Thunder.Models
{
    public class Audit
    {
        public DateTime? CreatedDate { set; get; }
        public DateTime? UpdatedDate { set; get; }
        [DefaultValue(1)]
        public int IsExist { set; get; }
    }
}
