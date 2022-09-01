using Thunder.Models;

namespace Thunder.ViewModel
{
    public class AccreditationRank
    {
        public AccreditationRank(int id, Accreditation homeAccreditation, Accreditation awayAccreditation, double score)
        {
            Id = id;
            HomeAccreditation = homeAccreditation;
            AwayAccreditation = awayAccreditation;
            Score = score;
        }

        public int Id { set; get; }
        public Accreditation HomeAccreditation { set; get; }
        public Accreditation AwayAccreditation { set; get; }
        public double Score { set; get; }
    }
}
