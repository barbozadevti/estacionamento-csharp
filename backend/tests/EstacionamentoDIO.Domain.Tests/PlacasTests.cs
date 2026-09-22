using EstacionamentoDIO.Domain;

namespace EstacionamentoDIO.Domain.Tests;

public class PlacasTests
{
    [Theory]
    [InlineData("ABC1234")] // formato antigo
    [InlineData("abc1234")]
    [InlineData("ABC1D23")] // formato Mercosul
    [InlineData(" ABC1234 ")]
    public void EhValida_ComFormatosAceitos_DeveRetornarTrue(string placa)
    {
        Assert.True(Placas.EhValida(placa));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    [InlineData("AB1234")]
    [InlineData("ABCD123")]
    [InlineData("1234ABC")]
    public void EhValida_ComFormatosInvalidos_DeveRetornarFalse(string? placa)
    {
        Assert.False(Placas.EhValida(placa));
    }

    [Fact]
    public void Normalizar_DeveTrimEUpperCase()
    {
        Assert.Equal("ABC1234", Placas.Normalizar(" abc1234 "));
    }
}
