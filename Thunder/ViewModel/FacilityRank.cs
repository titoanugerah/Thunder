using Thunder.Models;

namespace Thunder.ViewModel
{
    public class FacilityRank
    {
        public FacilityRank(int id, University university1, University university2 , double score)
        {
            Id = id;
            University1 = university1;
            University2 = university2;
            Score = score;
        }

        public int Id { get; set; }
        public University University1 { get; set; }
        public University University2 { get; set; }
        public double Score { get; set; }
    }
}
