using Microsoft.Extensions.DependencyInjection;

namespace RCL.SSL.CertificateBot.Core
{
    public static class CertificateBotExtension
    {
        public static IServiceCollection AddCertificateBotService(this IServiceCollection services,
            Action<CertificateBotOptions> setupAction)
        {
            services.AddTransient<IFileService, FileService>();
            services.AddTransient<ICertificateService, CertificateService>();
            services.AddTransient<ICertificateBotService, CertificateBotService>();
            services.AddTransient<IIISService, IISService>();
            services.Configure(setupAction);

            return services;
        }
    }
}
