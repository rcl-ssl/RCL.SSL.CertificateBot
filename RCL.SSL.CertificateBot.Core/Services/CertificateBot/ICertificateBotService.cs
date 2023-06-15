namespace RCL.SSL.CertificateBot.Core
{
    public interface ICertificateBotService
    {
        Task<string> InstallAndRenewCertificateAsync();
        Task<string> InstallAndRenewCertificateInIISAsync();
    }
}
