using EstacionamentoDIO.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EstacionamentoDIO.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? "Data Source=estacionamento.db";

        services.AddDbContext<EstacionamentoDbContext>(options => options.UseSqlite(connectionString));
        services.AddScoped<IVeiculoRepository, VeiculoRepository>();

        return services;
    }
}
