using Key.Du.Pay.BusinessLogic.CentroCusto;
using Key.Du.Pay.CrossCutting.ViewModel;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Key.Du.Pay.UserInterface.Pages.CentroCusto
{
    public class IndexModel : PageModel
    {
        private readonly ICentroCustoBll _bll;

        public IndexModel(ICentroCustoBll bll)
        {
            _bll = bll;
        }

        public List<CentroCustoVM> Itens { get; set; } = new();

        public async Task OnGetAsync()
        {
            Itens = await _bll.ListarAsync();
        }
    }
}
