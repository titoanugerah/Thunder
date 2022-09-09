namespace Thunder.ViewModel
{
    public class PriorityRank
    {
        public PriorityRank(int id, string priority1, string priority2, double score)
        {
            Id = id;
            Priority1 = priority1;
            Priority2 = priority2;
            Score = score;
//            ScoreString = scoreString;
        }

        public int Id { get; set; }
        public string Priority1 { get; set; }
        public string Priority2 { get; set; }
        public double Score { get; set; }
        public double NormalizationScore { set; get; }
        public string NormalizationScoreString { set; get; }

    }
}
