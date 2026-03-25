using Key.Du.Pay.BusinessLogic.Pagamento;
using Key.Du.Pay.CrossCutting.ViewModel;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Key.Du.Pay.UserInterface.Pages.Pagamento
{
    public class IndexModel : PageModel
    {
        private readonly IPagamentoBll _bll;

        public IndexModel(IPagamentoBll bll)
        {
            _bll = bll;
        }

        public List<PagamentoVM> Itens { get; set; } = new();

        public async Task OnGetAsync()
        {
            Itens = await _bll.ListarAsync();
        }
    }
}
