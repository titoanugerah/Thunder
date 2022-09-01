using Thunder.Models;

namespace Thunder.ViewModel
{
    public class FacilityRank
    {
        public FacilityRank(int id, University university1, int facilityUniversity1, University university2, int facilityUniversity2, double score)
        {
            Id = id;
            University1 = university1;
            FacilityUniversity1 = facilityUniversity1;
            University2 = university2;
            FacilityUniversity2 = facilityUniversity2;
            Score = score;
        }

        public int Id { get; set; }
        public University University1 { get; set; }
        public int FacilityUniversity1 { set; get; }
        public University University2 { get; set; }
        public int FacilityUniversity2 { set; get; }
        public double Score { get; set; }
    }
}
