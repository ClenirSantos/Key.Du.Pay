using Key.Du.Pay.BusinessLogic.PlanoPagamento;
using Key.Du.Pay.CrossCutting.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Key.Du.Pay.UserInterface.Pages.PlanoPagamento
{
    public class DeleteModel : PageModel
    {
        private readonly IPlanoPagamentoBll _bll;

        public DeleteModel(IPlanoPagamentoBll bll)
        {
            _bll = bll;
        }

        public PlanoPagamentoVM? Item { get; set; }

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
                TempData["Success"] = "Plano excluído.";
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
