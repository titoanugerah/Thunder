using System.ComponentModel;

namespace Thunder.Models
{
    public class Survey : Audit
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { set; get; }
        [DefaultValue(1)]
        public int PricePriority { set; get; }
        [DefaultValue(1)]
        public int FacilityPriority { set; get; }
        [DefaultValue(1)]
        public int CityPriority { set; get; }
        [DefaultValue(1)]
        public int AccreditationPriority { set; get; }

    }
}
