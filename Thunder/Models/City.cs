namespace Thunder.Models
{
    public class City : Audit
    {
        public int Id { get; set; }
        public string Name { get; set; }    
        public int ProvinceId { set; get; }
        public Province Province { set; get; }
    }
}
