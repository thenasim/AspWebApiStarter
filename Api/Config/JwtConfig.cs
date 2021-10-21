namespace Api.Config
{
    public class JwtConfig
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string Key { get; set; }
        public double DurationInMinutes { get; set; }
    }
}