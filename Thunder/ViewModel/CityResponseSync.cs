namespace Thunder.ViewModel
{
    public class CityResponseSync
    {
        public string message { get; set; }
        public string error { get; set; } 
        public List<CityDataSync> data { get; set; } 
    }
}
