namespace EstacionamentoDIO.Domain.Entities;

/// <summary>
/// Formas de pagamento aceitas na saída. O fluxo é simulado (não há integração
/// com um gateway de pagamento real) — serve para registrar como o cliente
/// pagou, como qualquer sistema de estacionamento faria no caixa.
/// </summary>
public enum FormaPagamento
{
    Dinheiro,
    Pix,
    CartaoCredito,
    CartaoDebito,
    CarteiraDigital,
}
