using EstacionamentoDIO.Application;
using Microsoft.AspNetCore.Mvc;

namespace EstacionamentoDIO.Api.Controllers;

[ApiController]
[Route("api/relatorio")]
public class RelatorioController : ControllerBase
{
    private readonly IEstacionamentoAppService servico;

    public RelatorioController(IEstacionamentoAppService servico)
    {
        this.servico = servico;
    }

    /// <summary>
    /// Relatório de faturamento por período (padrão: dia atual, UTC), com total
    /// arrecadado e detalhamento por forma de pagamento. Atende a persona
    /// "gestor do estacionamento" do Lean Inception do projeto.
    /// </summary>
    [HttpGet("faturamento")]
    [ProducesResponseType(typeof(RelatorioFaturamentoDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<RelatorioFaturamentoDto>> ObterFaturamento(
        [FromQuery] DateTime? inicio,
        [FromQuery] DateTime? fim,
        CancellationToken cancellationToken)
    {
        var relatorio = await servico.ObterRelatorioFaturamentoAsync(
            inicio?.ToUniversalTime(),
            fim?.ToUniversalTime(),
            cancellationToken);

        return Ok(relatorio);
    }
}
