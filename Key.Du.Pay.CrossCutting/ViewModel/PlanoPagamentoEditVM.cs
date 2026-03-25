namespace Key.Du.Pay.CrossCutting.ViewModel
{
    public class PlanoPagamentoEditVM
    {
        public int Id { get; set; }

        public int ResponsavelFinanceiroID { get; set; }

        public int CentroCustoID { get; set; }

        public List<CobrancaEditItemVM> Cobrancas { get; set; } = new();
    }
}
