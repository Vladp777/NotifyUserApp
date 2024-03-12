using NotifyUserApp.Models;
using NotifyUserApp.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

[assembly: FunctionsStartup(typeof(NotifyUserApp.StartUp))]
namespace NotifyUserApp;
public class StartUp : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder.Services.AddScoped<IEmailService, EmailService>();
        builder.Services.AddScoped<IBlobService, BlobService>();
        builder.Services.AddLogging();
        builder.Services.AddOptions<BlobConfig>()
            .Configure<IConfiguration>((s, c) =>
            {
                c.GetSection("BlobConfig").Bind(s);
            });
    }

    public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
    {
        base.ConfigureAppConfiguration(builder);

        var context = builder.GetContext();

        builder.ConfigurationBuilder
            .AddJsonFile(Path.Combine(context.ApplicationRootPath, "appsettings.json"), optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

    }
}


