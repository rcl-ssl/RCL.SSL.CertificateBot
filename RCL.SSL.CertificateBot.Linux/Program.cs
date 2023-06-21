using RCL.SSL.CertificateBot.Linux;
using RCL.SSL.SDK;
using RCL.SSL.CertificateBot.Core;

IHost host = Host.CreateDefaultBuilder(args)
    .UseSystemd()
    .ConfigureServices((hostContext, services) =>
    {
        IConfiguration Configuration = hostContext.Configuration;

        services.AddRCLSDKService(options => Configuration.Bind(RCLSDKOptions.RCLSDK, options));
        services.AddRCLCertificateBotService(options => Configuration.Bind(CertificateBotOptions.CertificateBot, options));
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
