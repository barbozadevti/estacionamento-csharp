using EstacionamentoDIO.Application;
using Microsoft.AspNetCore.Mvc;

namespace EstacionamentoDIO.Api.Controllers;

[ApiController]
[Route("api/estacionamento")]
public class EstacionamentoController : ControllerBase
{
    private readonly IEstacionamentoAppService servico;

    public EstacionamentoController(IEstacionamentoAppService servico)
    {
        this.servico = servico;
    }

    /// <summary>Retorna as vagas disponíveis e os preços vigentes.</summary>
    [HttpGet("status")]
    [ProducesResponseType(typeof(StatusEstacionamentoDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<StatusEstacionamentoDto>> ObterStatus(CancellationToken cancellationToken)
    {
        var status = await servico.ObterStatusAsync(cancellationToken);
        return Ok(status);
    }
}
