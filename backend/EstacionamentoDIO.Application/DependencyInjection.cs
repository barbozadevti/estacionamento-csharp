using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EstacionamentoDIO.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<EstacionamentoOptions>(configuration.GetSection(EstacionamentoOptions.SectionName));
        services.Configure<EstabelecimentoOptions>(configuration.GetSection(EstabelecimentoOptions.SectionName));
        services.AddSingleton<IRelogio, RelogioSistema>();
        services.AddScoped<IEstacionamentoAppService, EstacionamentoAppService>();

        return services;
    }
}
