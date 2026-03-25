using Key.Du.Pay.BusinessLogic.Cobranca;
using Key.Du.Pay.CrossCutting.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Key.Du.Pay.UserInterface.Pages.Cobranca
{
    public class IndexModel : PageModel
    {
        private readonly ICobrancaBll _bll;

        public IndexModel(ICobrancaBll bll)
        {
            _bll = bll;
        }

        public List<CobrancaVM> Itens { get; set; } = new();

        public async Task OnGetAsync()
        {
            Itens = await _bll.ListarAsync();
        }

        public async Task<IActionResult> OnPostCancelarAsync(int id)
        {
            var cob = await _bll.ObterPorIdAsync(id);
            if (cob == null)
                return NotFound();

            if (cob.Status != Key.Du.Pay.CrossCutting.Enum.EnumCobrancaStatus.Cancelada && 
                cob.Status != Key.Du.Pay.CrossCutting.Enum.EnumCobrancaStatus.Paga)
            {
                cob.Status = Key.Du.Pay.CrossCutting.Enum.EnumCobrancaStatus.Cancelada;
                await _bll.AtualizarAsync(cob);
                TempData["Success"] = "Cobrança cancelada com sucesso.";
            }

            return RedirectToPage();
        }
    }
}
