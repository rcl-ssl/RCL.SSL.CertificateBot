using RCL.SSL.CertificateBot.Core;
using RCL.SSL.CertificateBot.Windows;
using RCL.SSL.SDK;

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService()
    .ConfigureLogging((_, logging) => logging.AddEventLog())
    .ConfigureServices((hostContext, services) =>
    {
        IConfiguration Configuration = hostContext.Configuration;

        services.Configure<RCLSDKOptions>(Configuration.GetSection(RCLSDKOptions.RCLSDK));
        services.Configure<CertificateBotOptions>(Configuration.GetSection(CertificateBotOptions.CertificateBot));
        services.AddRCLSDKService();
        services.AddRCLCertificateBotService();
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
