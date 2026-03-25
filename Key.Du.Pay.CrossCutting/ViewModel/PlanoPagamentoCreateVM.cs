using Key.Du.Pay.CrossCutting.Enum;

namespace Key.Du.Pay.CrossCutting.ViewModel
{
    public class PlanoPagamentoCreateVM
    {
        public int ResponsavelId { get; set; }

        public int CentroDeCusto { get; set; }

        public List<CobrancaPlanoItemVM> Cobrancas { get; set; } = new();
    }

    public class CobrancaPlanoItemVM
    {
        public decimal Valor { get; set; }

        public DateTime DataVencimento { get; set; }

        public EnumMetodoPagamento MetodoPagamento { get; set; }

        public string? Descricao { get; set; }
    }
}
