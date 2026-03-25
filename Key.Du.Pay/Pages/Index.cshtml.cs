using Key.Du.Pay.BusinessLogic.CentroCusto;
using Key.Du.Pay.BusinessLogic.Cobranca;
using Key.Du.Pay.BusinessLogic.ResponsavelFinanceiro;
using Key.Du.Pay.CrossCutting.Enum;
using Key.Du.Pay.CrossCutting.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Key.Du.Pay.UserInterface.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ICentroCustoBll _centroCustoBll;
        private readonly IResponsavelFinanceiroBll _responsavelFinanceiroBll;
        private readonly ICobrancaBll _cobrancaBll;

        public IndexModel(
            ICentroCustoBll centroCustoBll,
            IResponsavelFinanceiroBll responsavelFinanceiroBll,
            ICobrancaBll cobrancaBll)
        {
            _centroCustoBll = centroCustoBll;
            _responsavelFinanceiroBll = responsavelFinanceiroBll;
            _cobrancaBll = cobrancaBll;
        }

        public List<CentroCustoVM>? CentroCustosVM { get; set; }
        public List<ResponsavelFinanceiroVM>? ResponsavelFinanceiroVM { get; set; }

        // Cobranças Canceladas
        public int CanceladasQtd { get; set; }
        public decimal CanceladasTotal { get; set; }

        // Cobranças Pagas
        public int PagasQtd { get; set; }
        public decimal PagasTotal { get; set; }

        // Cobranças Pagas Parcialmente
        public int PagasParcialmenteQtd { get; set; }
        public decimal PagasParcialmenteTotal { get; set; }

        public async Task<IActionResult> OnGetAsync([FromRoute] string permalink)
        {
            CentroCustosVM = await _centroCustoBll.ListarAsync();
            ResponsavelFinanceiroVM = await _responsavelFinanceiroBll.ListarAsync();

            var cobranças = await _cobrancaBll.ListarAsync();

            var canceladas = cobranças.Where(c => c.Status == EnumCobrancaStatus.Cancelada).ToList();
            CanceladasQtd = canceladas.Count;
            CanceladasTotal = canceladas.Sum(c => c.Valor);

            var pagas = cobranças.Where(c => c.Status == EnumCobrancaStatus.Paga).ToList();
            PagasQtd = pagas.Count;
            PagasTotal = pagas.Sum(c => c.Valor);

            var pagasParcialmente = cobranças.Where(c => c.Status == EnumCobrancaStatus.PagaParcialmente).ToList();
            PagasParcialmenteQtd = pagasParcialmente.Count;
            PagasParcialmenteTotal = pagasParcialmente.Sum(c => c.Valor);

            return Page();
        }
    }
}
