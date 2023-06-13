using RCL.SSL.CertificateBot.IIS;
using RCL.SSL.SDK;
using RCL.SSL.CertificateBot.Core;

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService()
    .ConfigureLogging((_, logging) => logging.AddEventLog())
    .ConfigureServices((hostContext, services) =>
    {
        IConfiguration Configuration = hostContext.Configuration;

        services.Configure<RCLSDKOptions>(Configuration.GetSection("RCLSDK"));
        services.AddRCLSDKService(options => { } );
        services.Configure<CertificateBotOptions>(Configuration.GetSection("CertificateBot"));
        services.AddCertificateBotService(options => { });
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
