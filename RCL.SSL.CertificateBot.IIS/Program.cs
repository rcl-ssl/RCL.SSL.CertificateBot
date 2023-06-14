using RCL.SSL.CertificateBot.IIS;
using RCL.SSL.SDK;
using RCL.SSL.CertificateBot.Core;

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
