using Thunder.Models;

namespace Thunder.ViewModel
{
    public class TuitionFeeRank
    {
        public TuitionFeeRank(int id, University university1, University university2, double score)
        {
            Id = id;
            University1 = university1;
            University2 = university2;
            Score = score;
        }

        public int Id { set; get; }
        public University University1 { set; get; } 
        public University University2 { set; get; } 
        public double Score { set; get; } 
    }
}
