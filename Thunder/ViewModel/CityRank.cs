using Thunder.Models;

namespace Thunder.ViewModel
{
    public class CityRank
    {
        public CityRank(int id, City homeCity, City awayCity)
        {
            Id = id;
            HomeCity = homeCity;
            AwayCity = awayCity;
            Score = homeCity.EducationIndexScore / awayCity.EducationIndexScore;
        }

        public int Id { get; set; }
        public City HomeCity { get; set; }
        public City AwayCity { get; set; }
        public double Score { set; get; }
    }
}
