using Thunder.Models;

namespace Thunder.ViewModel
{
    public class CityRank
    {
        public CityRank(int id, City city1, City city2, double score)
        {
            Id = id;
            City1 = city1;
            City2 = city2;
            Score = score;
        }

        public int Id { get; set; }
        public City City1 { get; set; }
        public City City2 { get; set; }
        public double Score { set; get; }
    }
}
