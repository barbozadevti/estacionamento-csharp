using EstacionamentoDIO.Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EstacionamentoDIO.Api.Controllers;

[ApiController]
[Route("api/estabelecimento")]
public class EstabelecimentoController : ControllerBase
{
    private readonly EstabelecimentoOptions opcoes;

    public EstabelecimentoController(IOptions<EstabelecimentoOptions> opcoes)
    {
        this.opcoes = opcoes.Value;
    }

    /// <summary>Retorna o nome e o CNPJ do estabelecimento onde o estacionamento opera.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(EstabelecimentoDto), StatusCodes.Status200OK)]
    public ActionResult<EstabelecimentoDto> Obter()
    {
        return Ok(new EstabelecimentoDto(opcoes.Nome, opcoes.Cnpj));
    }
}
