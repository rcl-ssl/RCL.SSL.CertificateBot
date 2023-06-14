using RCL.SSL.CertificateBot.Linux;
using RCL.SSL.SDK;
using RCL.SSL.CertificateBot.Core;

IHost host = Host.CreateDefaultBuilder(args)
    .UseSystemd()
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
