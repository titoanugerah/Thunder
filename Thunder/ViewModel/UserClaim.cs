namespace Thunder.ViewModel
{
    public class UserClaim
    {
        public string Issuer { set; get; }
        public string OriginalIssuer { set; get; }
        public string Type { set; get; }
        public string Value { set; get; }
        public string Name
        {
            get
            {
                if (Type != null)
                {
                    return Type.Split("/").ToList().LastOrDefault();
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
