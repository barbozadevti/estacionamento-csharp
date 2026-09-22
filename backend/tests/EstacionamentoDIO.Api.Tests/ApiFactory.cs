using EstacionamentoDIO.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EstacionamentoDIO.Api.Tests;

public class ApiFactory : WebApplicationFactory<Program>, IDisposable
{
    private readonly SqliteConnection conexao = new("DataSource=:memory:");

    protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
    {
        conexao.Open();

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<EstacionamentoDbContext>>();
            services.AddDbContext<EstacionamentoDbContext>(options => options.UseSqlite(conexao));
        });
    }

    public new void Dispose()
    {
        conexao.Dispose();
        base.Dispose();
        GC.SuppressFinalize(this);
    }
}
