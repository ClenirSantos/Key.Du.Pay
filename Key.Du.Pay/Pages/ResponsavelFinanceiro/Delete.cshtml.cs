using Key.Du.Pay.BusinessLogic.ResponsavelFinanceiro;
using Key.Du.Pay.CrossCutting.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Key.Du.Pay.UserInterface.Pages.ResponsavelFinanceiro
{
    public class DeleteModel : PageModel
    {
        private readonly IResponsavelFinanceiroBll _bll;

        public DeleteModel(IResponsavelFinanceiroBll bll)
        {
            _bll = bll;
        }

        public ResponsavelFinanceiroVM? Item { get; set; }

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
                TempData["Success"] = "Responsável excluído.";
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
