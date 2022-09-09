using Thunder.Models;

namespace Thunder.ViewModel
{
    public class FinderResult
    {
        public FinderResult(University university, double finalScore)
        {
            University = university;
            FinalScore = finalScore;
        }

        public University University { set; get; }
        public double FinalScore { set; get; }
    }
}
