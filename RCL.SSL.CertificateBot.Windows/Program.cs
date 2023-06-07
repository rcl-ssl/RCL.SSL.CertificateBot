using RCL.SSL.CertificateBot.Core;
using RCL.SSL.CertificateBot.Windows;
using RCL.SSL.SDK;

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService()
    .ConfigureLogging((_, logging) => logging.AddEventLog())
    .ConfigureServices((hostContext, services) =>
    {
        IConfiguration Configuration = hostContext.Configuration;

        services.AddRCLSDKService(options => Configuration.Bind("RCLSDK", options));
        services.AddCertificateBotService(options => Configuration.Bind("CertificateBot", options));

        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
