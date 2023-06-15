using RCL.SSL.SDK;

namespace RCL.SSL.CertificateBot.Core
{
    public interface ICertificateService
    {
        Task GetTestAsync();
        Task<Certificate> GetCertificateAsync(Certificate certificate);
        Task<List<Certificate>> GetCertificatesToRenewAsync();
        Task RenewCertificateAsync(Certificate certificate);
    }
}
