using Key.Du.Pay.BusinessLogic.PlanoPagamento;
using Key.Du.Pay.CrossCutting.ViewModel;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Key.Du.Pay.UserInterface.Pages.PlanoPagamento
{
    public class IndexModel : PageModel
    {
        private readonly IPlanoPagamentoBll _bll;

        public IndexModel(IPlanoPagamentoBll bll)
        {
            _bll = bll;
        }

        public List<PlanoPagamentoVM> Itens { get; set; } = new();

        public async Task OnGetAsync()
        {
            Itens = await _bll.ListarAsync();
        }
    }
}
