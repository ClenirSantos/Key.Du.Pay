using Key.Du.Pay.CrossCutting.Enum;

namespace Key.Du.Pay.BusinessLogic.Cobranca
{
    internal static class CobrancaCodigoGerador
    {
        public static double Gerar(EnumMetodoPagamento metodo)
        {
            return metodo switch
            {
                EnumMetodoPagamento.Boleto => Random.Shared.NextInt64(1_000_000_000_000_000, 9_999_999_999_999_999),
                EnumMetodoPagamento.Pix => Random.Shared.Next(100_000_000, 999_999_999),
                _ => Random.Shared.Next(100_000, 999_999_999)
            };
        }
    }
}
