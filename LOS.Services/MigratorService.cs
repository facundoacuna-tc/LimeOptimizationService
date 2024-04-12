using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

using Dapper;
using FluentMigrator.Runner;

using LOS.Common.ExtensionMethods;
using LOS.Common.Settings;
using LOS.Services.Interfaces;


namespace LOS.Services
{
    public class MigratorService : IMigratorService
    {
        private IMigrationRunner runner;
        private IConfiguration cfg;

        public MigratorService(IMigrationRunner runner, IConfiguration cfg)
        {
            this.runner = runner;
            this.cfg = cfg;
        }

        private void EnsureDatabase()
        {
            var cs = cfg.GetConnectionString(AppSettings.Settings.UseDatabase);
            var dbName = cs.RightOf("Database=").LeftOf(";");
            var master = cfg.GetConnectionString("MasterConnection");

            //var dbName = "LimeServiceDb";
            //var master = "Server=localhost;Database=master;Integrated Security=True;";

            var parameters = new DynamicParameters();
            parameters.Add("name", dbName);

            using var connection = new SqlConnection(master);
            var records = connection.Query("SELECT name FROM sys.databases WHERE name = @name", parameters);

            if (!records.Any())
            {
                connection.Execute($"CREATE DATABASE [{dbName}]");
            }
        }


        private string ConsoleHook(Action action)
        {
            var saved = Console.Out;
            var sb = new StringBuilder();
            var tw = new StringWriter(sb);
            Console.SetOut(tw);

            try
            {
                action();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            tw.Close();

            // Restore the default console out.
            // Simpler: https://stackoverflow.com/a/26095640
            Console.SetOut(saved);

            var errs = sb.ToString();

            return errs;
        }

        public string MigrateUp()
        {
            EnsureDatabase();

            var errs = ConsoleHook(() => runner.MigrateUp());
            var result = String.IsNullOrEmpty(errs) ? "Success" : errs;

            return result;
        }

        public string MigrateDown(long version)
        {
            var errs = ConsoleHook(() => runner.MigrateDown(version));
            var result = String.IsNullOrEmpty(errs) ? "Success" : errs;

            return result;
        }

    }
}
