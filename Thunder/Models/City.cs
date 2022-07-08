namespace Thunder.Models
{
    public class City
    {
        public int Id { get; set; }
        public string Name { get; set; }    
        public int ProvinceId { set; get; }
        public Province Province { set; get; }
    }
}
