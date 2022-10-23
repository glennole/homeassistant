using HomeAssistant.Service;
using HomeAssistant.Service.Vault;
using Microsoft.Extensions.Options;
using Quartz;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, configuration) =>
    {
        configuration.Sources.Clear();
        IHostEnvironment env = hostingContext.HostingEnvironment;
        configuration
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        configuration.AddEnvironmentVariables(prefix: "VAULT_");
        var buildConfig = configuration.Build();
       
        configuration.AddVault(options =>
        {
            var vaultOptions = buildConfig.GetSection("Vault");
            options.Address = vaultOptions["Address"];
            options.MountPath = vaultOptions["MountPath"];
            options.Token = buildConfig.GetSection("TOKEN").Value;
        });
        
    })
    .ConfigureServices((context, services) =>
    {
        var configurationRoot = context.Configuration;
        services.Configure<VaultOptions>(configurationRoot.GetSection("Vault"));
        if (configurationRoot["HomeAssistantOptions:Token"] != null)
        {
            Console.WriteLine(configurationRoot["HomeAssistantOptions:Token"]);
        }
        
        services.Configure<HomeAssistantOptions>(
            configurationRoot.GetSection(nameof(HomeAssistantOptions)));
        
        services.AddSingleton<IHomeAssistantProxy, HomeAssistantProxy>();
        
        services.AddQuartz(q =>
        {
            q.UseMicrosoftDependencyInjectionJobFactory();
            q.ScheduleJob<WaterHeaterJob>(trigger => trigger
                .WithIdentity("ControlWaterHeaterTrigger")
                .WithSimpleSchedule(s => s.WithIntervalInSeconds(15)
                    .RepeatForever())
                .WithDescription("This trigger will run every 15 seconds to turn on or off water heater.")
            );
        });
        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });
        services.AddTransient<WaterHeaterJob>();
        services.AddHostedService<VVSBackgroundService>();
    })
    .Build();

IConfiguration config = host.Services.GetRequiredService<IConfiguration>();

Console.WriteLine(config["HomeAssistant:Token"]);
await host.RunAsync();