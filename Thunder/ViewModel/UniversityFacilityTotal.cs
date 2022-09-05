using Thunder.Models;

namespace Thunder.ViewModel
{
    public class UniversityFacilityTotal
    {
        public UniversityFacilityTotal(University university, int total)
        {
            University = university;
            Total = total;
        }
        public University University { set; get; }
        public int Total { set; get; }
    }
}
