using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.eShopWeb.Infrastructure.Data;
using Microsoft.eShopWeb.Infrastructure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Microsoft.eShopWeb.Infrastructure;

public static class Dependencies
{
    public static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
    {
        bool useOnlyInMemoryDatabase = false;
        if (configuration["UseOnlyInMemoryDatabase"] != null)
        {
            useOnlyInMemoryDatabase = bool.Parse(configuration["UseOnlyInMemoryDatabase"]!);
        }

        if (useOnlyInMemoryDatabase)
        {
            services.AddDbContext<CatalogContext>(c =>
               c.UseInMemoryDatabase("Catalog"));

            services.AddDbContext<AppIdentityDbContext>(c =>
                c.UseInMemoryDatabase("Identity"));
        }
        else
        {
            // use real database
            // Requires LocalDB which can be installed with SQL Server Express 2016
            // https://www.microsoft.com/en-us/download/details.aspx?id=54284
            services.AddDbContext<CatalogContext>(c =>
            {
                string connectionString = configuration.GetConnectionString("CatalogConnection");

                if (connectionString?.StartsWith("postgres://") ?? false)
                {
                    c.UseNpgsql(ConvertPostgresUriToConnectionString(connectionString));
                }
                else
                {
                    c.UseNpgsql(connectionString);
                }
            });

            // Add Identity DbContext
            services.AddDbContext<AppIdentityDbContext>(c =>
            {
                string connectionString = configuration.GetConnectionString("IdentityConnection");

                if (connectionString?.StartsWith("postgres://") ?? false)
                {
                    c.UseNpgsql(ConvertPostgresUriToConnectionString(connectionString));
                }
                else
                {
                    c.UseNpgsql(connectionString);
                }
            });
        }
    }

    private static string ConvertPostgresUriToConnectionString(string postgresUri)
    {
        var databaseUri = new Uri(postgresUri);
        var userInfo = databaseUri.UserInfo.Split(':');

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = databaseUri.Host,
            Port = databaseUri.Port > 0 ? databaseUri.Port : 5432,
            Username = userInfo[0],
            Password = userInfo[1],
            Database = databaseUri.LocalPath.TrimStart('/')
        };

        return builder.ToString();
    }
}
