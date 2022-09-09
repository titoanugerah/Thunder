using Thunder.Models;

namespace Thunder.ViewModel
{
    public class FinalResult
    {
        public FinalResult (int id, University university, double scoreCity, double scoreFacility, double scoreAccreditation, double scoreTuitionFee, List<Priority> priorities)
        {
            Id = id;
            University = university;
            ScoreCity = scoreCity * priorities.Where(x => x.Name == "city").First().Weight;
            ScoreFacility = scoreFacility * priorities.Where(x => x.Name == "facility").First().Weight;
            ScoreAccreditation = scoreAccreditation * priorities.Where(x => x.Name == "accreditation").First().Weight;
            ScoreTuitionFee = scoreTuitionFee * priorities.Where(x => x.Name == "price").First().Weight;
            ScoreTotal = Math.Round(ScoreCity + ScoreFacility + ScoreAccreditation + ScoreTuitionFee, 2, MidpointRounding.AwayFromZero) ;
        }

        public int Id { set; get; }
        public University University { set; get; }
        public double ScoreCity { set; get; }
        public double ScoreFacility { set; get; }
        public double ScoreAccreditation { set; get; }
        public double ScoreTuitionFee { set; get; }
        public double ScoreTotal { set; get; }
    }
}
