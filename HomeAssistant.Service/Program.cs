using System.Collections.Specialized;
using HomeAssistant.Contracts.Repositories;
using HomeAssistant.Database;
using HomeAssistant.PostgreSql.Repositories;
using HomeAssistant.Service;
using HomeAssistant.Service.Configuration;
using HomeAssistant.Service.Vault;
using Quartz;
using Serilog;
using Serilog.Formatting.Compact;
using Serilog.Sinks.Grafana.Loki;
using ILogger = Serilog.ILogger;


IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        ILogger logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .CreateLogger();
    })
    .UseSerilog((ctx, cfg) =>
    {
        cfg.Enrich.WithProperty("Application", ctx.HostingEnvironment.ApplicationName)
            .Enrich.WithProperty("Environment", ctx.HostingEnvironment.EnvironmentName)
            .ReadFrom.Configuration(ctx.Configuration);
    })
    .ConfigureAppConfiguration((hostingContext, configuration) =>
    {
        configuration.Sources.Clear();
        IHostEnvironment env = hostingContext.HostingEnvironment;
        configuration
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
        configuration
            .AddJsonFile($"appsettings.{env}.json", true, true);
        configuration.AddEnvironmentVariables(prefix: "VAULT_");
        configuration.AddEnvironmentVariables();
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

        var waterHeaterCronExp = configurationRoot.GetSection("Jobs:WaterHeater:CronExp");
        var nordpoolCronExp = configurationRoot.GetSection("Jobs:Nordpool:CronExp");
        
        services.Configure<HomeAssistantOptions>(
            configurationRoot.GetSection(nameof(HomeAssistantOptions)));

        services.Configure<PostgresqlOptions>(
            configurationRoot.GetSection(nameof(PostgresqlOptions)));
        
        services.AddSingleton<IHomeAssistantProxy, HomeAssistantProxy>();
        services.AddSingleton<IDailyHourPriceRepository, DailyHourPriceRepository>();
        services.AddSingleton<IHeavyDutySwitchRepository, HeavyDutySwitchRepository>();

        services.AddQuartz(q =>
        {
            q.UseMicrosoftDependencyInjectionJobFactory();
            q.ScheduleJob<WaterHeaterJob>(trigger => trigger
                .WithIdentity("ControlWaterHeaterTrigger")
                .WithCronSchedule(waterHeaterCronExp.Value, x => x.InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Europe/Oslo")))
                .WithDescription("This trigger will run every 15 seconds to turn on or off water heater.")
            );
            q.ScheduleJob<NordpoolSensorJob>(trigger => trigger
                .WithIdentity("GetTodayAndTomorrowsPrices")
                .WithCronSchedule(nordpoolCronExp.Value, x => x.InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Europe/Oslo")))
                .WithDescription(
                    "This trigger will fetch data from the Nordpool sensor and add the hour prices for today and tomorrow to the database.")
            );
        });
        services.AddQuartzHostedService(options => { options.WaitForJobsToComplete = true; });
        services.AddTransient<WaterHeaterJob>();
        services.AddTransient<NordpoolSensorJob>();
        services.AddHostedService<VVSBackgroundService>();
    })
    .Build();

IConfiguration config = host.Services.GetRequiredService<IConfiguration>();
await host.RunAsync();