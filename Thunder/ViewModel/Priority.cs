namespace Thunder.ViewModel
{
    public class Priority
    {
        public Priority(int id, string name, int score = 0)
        {
            Id = id;
            Name = name;
            Score = score;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public int Score { set; get; }
        public double SumPairWise { set; get; }
        public double Weight { set; get; }
    }
}
