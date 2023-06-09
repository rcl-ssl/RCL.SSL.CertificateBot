﻿using Microsoft.Extensions.DependencyInjection;

namespace RCL.SSL.CertificateBot.Core
{
    public static class CertificateBotExtension
    {
        public static IServiceCollection AddRCLCertificateBotService(this IServiceCollection services,
            Action<CertificateBotOptions> setupAction)
        {
            services.AddTransient<IFileService, FileService>();
            services.AddTransient<ICertificateService, CertificateService>();
            services.AddTransient<ICertificateBotService, CertificateBotService>();
            services.AddTransient<IIISService, IISService>();
            services.Configure<CertificateBotOptions>(setupAction);

            return services;
        }
    }
}
