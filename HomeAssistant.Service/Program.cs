using HomeAssistant.Contracts.Repositories;
using HomeAssistant.Database;
using HomeAssistant.PostgreSql.Repositories;
using HomeAssistant.Service;
using HomeAssistant.Service.Configuration;
using HomeAssistant.Service.HvaKosterStrommen;
using HomeAssistant.Service.Jobs;
using HomeAssistant.Service.SendGrid;
using HomeAssistant.Service.Services;
using HomeAssistant.Service.Vault;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Quartz;
using Serilog;
using ILogger = Serilog.ILogger;


var builder = WebApplication.CreateBuilder(args);
builder.Host.ConfigureLogging(logging =>
{
    logging.ClearProviders();
    ILogger logger = new LoggerConfiguration()
        .Enrich.FromLogContext()
        .CreateLogger();
});
builder.Host.UseSerilog((ctx, cfg) =>
{
    cfg.Enrich.WithProperty("Application", ctx.HostingEnvironment.ApplicationName)
        .Enrich.WithProperty("Environment", ctx.HostingEnvironment.EnvironmentName)
        .ReadFrom.Configuration(ctx.Configuration);
});
builder.Host.ConfigureAppConfiguration((hostingContext, configuration) =>
{
    configuration.Sources.Clear();
    IHostEnvironment env = hostingContext.HostingEnvironment;
    configuration
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
    configuration
        .AddJsonFile($"appsettings.{env}.json", true, true);
    configuration.AddEnvironmentVariables();
    var buildConfig = configuration.Build();

    AzureKeyVaultOptions azureKeyVaultOptions = new AzureKeyVaultOptions();
    buildConfig.GetSection(AzureKeyVaultOptions.AzureKeyVault).Bind(azureKeyVaultOptions);

    configuration.AddAzureKeyVault(options =>
    {
        options.ClientId = azureKeyVaultOptions.ClientId;
        options.ClientSecret = azureKeyVaultOptions.ClientSecret;
        options.SecretName = azureKeyVaultOptions.SecretName;
        options.TenantId = azureKeyVaultOptions.TenantId;
        options.KeyVaultName = azureKeyVaultOptions.KeyVaultName;
    });
});



builder.Host.ConfigureServices((context, services) =>
{
    var configurationRoot = context.Configuration;
    //services.Configure<VaultOptions>(configurationRoot.GetSection("Vault"));

    var waterHeaterCronExp = configurationRoot.GetSection("Jobs:WaterHeater:CronExp");
    var nordpoolCronExp = configurationRoot.GetSection("Jobs:Nordpool:CronExp");
    var sendGridApiKey = configurationRoot.GetSection("SendGrid:ApiKey");

    services.Configure<HomeAssistantOptions>(
        configurationRoot.GetSection(HomeAssistantOptions.HomeAssistant));

    services.Configure<PostgresqlOptions>(
        configurationRoot.GetSection(PostgresqlOptions.Postgresql));
    
    DatabaseWorker.Migrate(configurationRoot.GetSection(PostgresqlOptions.Postgresql).Get<PostgresqlOptions>().ConnectionString);
    
    services.AddSingleton<IHomeAssistantProxy, HomeAssistantProxy>();
    services.AddSingleton<IDailyHourPriceRepository, DailyHourPriceRepository>();
    services.AddSingleton<IHeavyDutySwitchRepository, HeavyDutySwitchRepository>();
    services.AddSingleton<IEmailService, EmailService>();
    services.AddSingleton<IWaterHeaterService, WaterHeaterService>();
    services.AddSingleton<IHvaKosterStrommenHourPriceService, HvaKosterStrommenHourPriceService>();
    services.AddSingleton<IDailyHourPriceService, DailyHourPriceService>();

    services.AddEndpointsApiExplorer();
    
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "HomeAssistantAPI", Version = "v1.0.0" });
    
        var securitySchema = new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        };
    
        c.AddSecurityDefinition("Bearer", securitySchema);
    
        var securityRequirement = new OpenApiSecurityRequirement
        {
            { securitySchema, new[] { "Bearer" } }
        };
    
        c.AddSecurityRequirement(securityRequirement);
    });

    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, c =>
        {
            c.Authority = $"https://{builder.Configuration["Auth0:Domain"]}";
            c.TokenValidationParameters = new TokenValidationParameters
            {
                ValidAudience = builder.Configuration["Auth0:Audience"],
                ValidIssuer = $"{builder.Configuration["Auth0:Domain"]}"
            };
        });

    services.AddAuthorization(o =>
    {
        o.AddPolicy("homeassistant:read-write", p => 
            p.RequireAuthenticatedUser().RequireClaim("scope", "homeassistant:read-write")
        );
    });
    
    services.AddQuartz(q =>
    {
        q.UseMicrosoftDependencyInjectionJobFactory();
        q.ScheduleJob<WaterHeaterJob>(trigger => trigger
            .WithIdentity("ControlWaterHeaterTrigger")
            .WithCronSchedule(waterHeaterCronExp.Value,
                x => x.InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Europe/Oslo")))
            .WithDescription("This trigger will run every 15 seconds to turn on or off water heater.")
        );
        q.ScheduleJob<NordpoolSensorJob>(trigger => trigger
            .WithIdentity("GetTodayAndTomorrowsPrices")
            .WithCronSchedule(nordpoolCronExp.Value,
                x => x.InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Europe/Oslo")))
            .WithDescription(
                "This trigger will fetch data from the Nordpool sensor and add the hour prices for today and tomorrow to the database.")
        );
        q.ScheduleJob<SendConsumptionReportJob>(trigger => trigger
            .WithIdentity("SendConsumptionReport")
            .WithCronSchedule("0 2 2 * * ?", x => x.InTimeZone(TimeZoneInfo.FindSystemTimeZoneById("Europe/Oslo")))
            .WithDescription(
                "This trigger will send a consumption report by SendGrid API")
        );
    });
    services.AddQuartzHostedService(options => { options.WaitForJobsToComplete = true; });
    services.AddTransient<WaterHeaterJob>();
    services.AddTransient<NordpoolSensorJob>();
});
WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();


app.RegisterWaterHeaterAPIs();
app.MapGet("/spotprices/{date}", (DateTime date, IDailyHourPriceRepository priceRepository) => priceRepository.HasPricesForGivenDate(date))
    .RequireAuthorization();
app.MapPost("/spotprices/sync",
        (IDailyHourPriceService dailyHourPriceService) => dailyHourPriceService.FetchAndStoreMissingDailyHourPrices())
    .RequireAuthorization();

IConfiguration config = app.Services.GetRequiredService<IConfiguration>();

try
{
    await app.RunAsync();
}
catch (Exception e)
{
    Log.Error(e, "Homeassistant stopped due to error.");
}