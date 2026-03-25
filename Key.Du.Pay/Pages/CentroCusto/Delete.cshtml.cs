using Key.Du.Pay.BusinessLogic.CentroCusto;
using Key.Du.Pay.CrossCutting.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Key.Du.Pay.UserInterface.Pages.CentroCusto
{
    public class DeleteModel : PageModel
    {
        private readonly ICentroCustoBll _bll;

        public DeleteModel(ICentroCustoBll bll)
        {
            _bll = bll;
        }

        public CentroCustoVM? Item { get; set; }

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
                TempData["Success"] = "Centro de custo excluído.";
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
