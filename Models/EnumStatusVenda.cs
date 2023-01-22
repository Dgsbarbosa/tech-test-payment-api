using System.Text.Json.Serialization;

namespace tech_test_payment_api.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EnumStatusVenda
    {
        PagamentoAprovado,
        EnviadoParaTransportadora,
        Entregue,
        Cancelada
    }
}