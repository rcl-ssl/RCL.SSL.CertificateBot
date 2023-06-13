#nullable disable

namespace RCL.SSL.CertificateBot.Core
{
    public class CertificateBotOptions
    {
        public string SaveCertificatePath { get; set; }
        public List<string> IncludeCertificates { get; set; }
        public List<string> IISBindings { get; set; }
    }
}
