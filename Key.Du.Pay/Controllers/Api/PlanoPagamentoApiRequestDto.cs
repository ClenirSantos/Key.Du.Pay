namespace Key.Du.Pay.UserInterface.Controllers.Api
{
    public class PlanoPagamentoApiRequestDto
    {
        public int ResponsavelId { get; set; }

        public int CentroDeCusto { get; set; }

        public List<CobrancaItemApiDto> Cobrancas { get; set; } = new();
    }

    public class CobrancaItemApiDto
    {
        public decimal Valor { get; set; }

        public DateTime DataVencimento { get; set; }

        public string MetodoPagamento { get; set; } = "";
    }

    public class PagamentoApiRequestDto
    {
        /// <summary>Ignorado quando <see cref="PagamentoIntegral"/> é true; nesse caso usa o saldo devedor.</summary>
        public decimal Valor { get; set; }

        public DateTime DataPagamento { get; set; }

        /// <summary>Se true, registra pagamento pelo saldo total da cobrança (quitação integral).</summary>
        public bool PagamentoIntegral { get; set; }
    }
}
