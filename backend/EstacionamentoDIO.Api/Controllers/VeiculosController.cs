using EstacionamentoDIO.Application;
using Microsoft.AspNetCore.Mvc;

namespace EstacionamentoDIO.Api.Controllers;

[ApiController]
[Route("api/veiculos")]
public class VeiculosController : ControllerBase
{
    private readonly IEstacionamentoAppService servico;

    public VeiculosController(IEstacionamentoAppService servico)
    {
        this.servico = servico;
    }

    /// <summary>Lista veículos. Use ?status=estacionados (padrão) ou ?status=historico.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<VeiculoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<VeiculoDto>>> Listar(
        [FromQuery] string status = "estacionados",
        CancellationToken cancellationToken = default)
    {
        var veiculos = status.Equals("historico", StringComparison.OrdinalIgnoreCase)
            ? await servico.ListarHistoricoAsync(cancellationToken)
            : await servico.ListarEstacionadosAsync(cancellationToken);

        return Ok(veiculos);
    }

    /// <summary>Registra a entrada de um veículo.</summary>
    [HttpPost("entrada")]
    [ProducesResponseType(typeof(VeiculoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<ActionResult<VeiculoDto>> RegistrarEntrada(
        [FromBody] RegistrarEntradaRequest request,
        CancellationToken cancellationToken)
    {
        var veiculo = await servico.RegistrarEntradaAsync(request.Placa, cancellationToken);
        return CreatedAtAction(nameof(Listar), veiculo);
    }

    /// <summary>Registra a saída de um veículo estacionado, calculando o valor a cobrar.</summary>
    [HttpPost("{placa}/saida")]
    [ProducesResponseType(typeof(RegistrarSaidaResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RegistrarSaidaResultDto>> RegistrarSaida(
        string placa,
        CancellationToken cancellationToken)
    {
        var resultado = await servico.RegistrarSaidaAsync(placa, cancellationToken);
        return Ok(resultado);
    }
}

public record RegistrarEntradaRequest(string Placa);
