using RCL.SSL.SDK;

namespace RCL.SSL.CertificateBot.Core
{
    internal interface ICertificateService
    {
        Task GetTestAsync();
        Task<Certificate> GetCertificateAsync(Certificate certificate);
        Task<List<Certificate>> GetCertificatesToRenewAsync();
        Task RenewCertificateAsync(Certificate certificate);
    }
}
