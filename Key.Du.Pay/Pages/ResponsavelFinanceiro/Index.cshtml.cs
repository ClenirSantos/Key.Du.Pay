using Key.Du.Pay.BusinessLogic.ResponsavelFinanceiro;
using Key.Du.Pay.CrossCutting.ViewModel;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Key.Du.Pay.UserInterface.Pages.ResponsavelFinanceiro
{
    public class IndexModel : PageModel
    {
        private readonly IResponsavelFinanceiroBll _bll;

        public IndexModel(IResponsavelFinanceiroBll bll)
        {
            _bll = bll;
        }

        public List<ResponsavelFinanceiroVM> Itens { get; set; } = new();

        public async Task OnGetAsync()
        {
            Itens = await _bll.ListarAsync();
        }
    }
}
