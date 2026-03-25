using Key.Du.Pay.BusinessLogic.Pagamento;
using Key.Du.Pay.CrossCutting.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Key.Du.Pay.UserInterface.Pages.Pagamento
{
    public class DeleteModel : PageModel
    {
        private readonly IPagamentoBll _bll;

        public DeleteModel(IPagamentoBll bll)
        {
            _bll = bll;
        }

        public PagamentoVM? Item { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Item = await _bll.ObterPorIdAsync(id);
            if (Item == null)
                return NotFound();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            try
            {
                await _bll.ExcluirAsync(id);
                TempData["Success"] = "Pagamento excluído.";
                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToPage("Index");
            }
        }
    }
}
