namespace Thunder.ViewModel
{
    public class PriorityRank
    {
        public PriorityRank(int id, Priority priority1, Priority priority2, double score, string scoreString)
        {
            Id = id;
            Priority1 = priority1;
            Priority2 = priority2;
            Score = score;
            ScoreString = scoreString;
        }

        public int Id { get; set; }
        public Priority Priority1 { get; set; }
        public Priority Priority2 { get; set; }
        public double Score { get; set; }
        public string ScoreString { set; get; }
        public double NormalizationScore { set; get; }
        public string NormalizationScoreString { set; get; }

    }
}
