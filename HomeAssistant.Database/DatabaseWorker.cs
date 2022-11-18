using System.Reflection;
using DbUp;
using Serilog;

namespace HomeAssistant.Database;

public static class DatabaseWorker
{
    public static int Migrate(string connectionString)
    {
        EnsureDatabase.For.PostgresqlDatabase(connectionString);

        var upgrader =
            DeployChanges.To
                .PostgresqlDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                .LogToConsole()
                .Build();

        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
        {
            Log.Error(result.Error, result.Error.Message);
            return -1;
        }
        else
        {
            foreach (var resultScript in result.Scripts)
            {
                Log.Information($"Successfully added {resultScript.Name}");
            }
            Log.Information($"HomeAssistant was successfully upgraded with {result.Scripts.Count()} migration scripts.");
        }

        return 0;
    }
}