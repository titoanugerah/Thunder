using Thunder.Models;

namespace Thunder.ViewModel
{
    public class FinalResult
    {
        public FinalResult (int id, University university, double scoreCity, double scoreFacility, double scoreAccreditation, List<Priority> priorities)
        {
            Id = id;
            University = university;
            ScoreCity = scoreCity * priorities.Where(x => x.Name == "City").First().Weight;
            ScoreFacility = scoreFacility * priorities.Where(x => x.Name == "Facility").First().Weight;
            ScoreAccreditation = scoreAccreditation * priorities.Where(x => x.Name == "Accreditation").First().Weight;
            ScoreTotal = (ScoreCity + ScoreFacility + ScoreAccreditation) / university.TuitionFee;
        }

        public int Id { set; get; }
        public University University { set; get; }
        public double ScoreCity { set; get; }
        public double ScoreFacility { set; get; }
        public double ScoreAccreditation { set; get; }
        public double ScoreTotal { set; get; }
    }
}
