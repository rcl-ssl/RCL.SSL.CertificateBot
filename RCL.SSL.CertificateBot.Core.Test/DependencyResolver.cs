using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RCL.SSL.SDK;

namespace RCL.SSL.CertificateBot.Core.Test
{
    public static class DependencyResolver
    {
        public static ServiceProvider ServiceProvider()
        {
            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddUserSecrets<TestProject>();
            IConfiguration configuration = builder.Build();

            IServiceCollection services = new ServiceCollection();

            services.AddRCLSDKService(options => configuration.Bind("RCLSDK", options));
            services.AddRCLCertificateBotService(options => configuration.Bind("CertificateBot",options));

            return services.BuildServiceProvider();
        }
    }

    public class TestProject
    {
    }
}
