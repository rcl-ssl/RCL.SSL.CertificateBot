#nullable disable

namespace RCL.SSL.CertificateBot.Core
{
    public class CertificateBotOptions
    {
        public const string CertificateBot = "CertificateBot";

        public string SaveCertificatePath { get; set; }
        public List<string> IncludeCertificates { get; set; }
        public List<string> IISBindings { get; set; }
    }
}
