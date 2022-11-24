namespace WebApi
{
    public class EndpointConfiguration
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public string Scheme { get; set; }

        public string CertPath { get; set; }

        public string CertPassword { get; set; }
    }
}