using Key.Du.Pay.BusinessLogic.Cobranca;
using Key.Du.Pay.CrossCutting.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Key.Du.Pay.UserInterface.Pages.Cobranca
{
    public class DeleteModel : PageModel
    {
        private readonly ICobrancaBll _bll;

        public DeleteModel(ICobrancaBll bll)
        {
            _bll = bll;
        }

        public CobrancaVM? Item { get; set; }

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
                TempData["Success"] = "Cobrança excluída.";
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
