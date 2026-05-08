using ApiGenerico.Utils.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ApiGenerico.Infrastructure.Context
{
    public class ContextSqlFactory : IDesignTimeDbContextFactory<ContextSql>
    {
        public ContextSql CreateDbContext(string[] args)
        {
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "ApiGenerico.WebAPI");
            var appSettingsPath = Path.Combine(basePath, "appsettings.json");
            var appSettingsDevelopmentPath = Path.Combine(basePath, "appsettings.Development.json");
            var configuration = new ConfigurationBuilder()
                .AddJsonFile(appSettingsPath, optional: false)
                .AddJsonFile(appSettingsDevelopmentPath, optional: true)
                .AddEnvironmentVariables()
                .Build();

            var encryptedConnection = configuration["ConnectionStrings:ConnetionGenerico"];
            if (string.IsNullOrWhiteSpace(encryptedConnection))
            {
                throw new InvalidOperationException("No se encontro la cadena ConnetionGenerico en appsettings.json.");
            }

            var connectionString = new EncryptionService().Decrypt(encryptedConnection);
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("No fue posible desencriptar la cadena ConnetionGenerico.");
            }

            var optionsBuilder = new DbContextOptionsBuilder<ContextSql>();
            optionsBuilder.UseSqlServer(connectionString);

            return new ContextSql(optionsBuilder.Options, configuration);
        }
    }
}
