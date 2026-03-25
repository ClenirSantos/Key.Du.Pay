using Key.Du.Pay.BusinessLogic.Pagamento;
using Key.Du.Pay.CrossCutting.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
namespace Key.Du.Pay.UserInterface.Pages.Pagamento
{
    public class EditModel : PageModel
    {
        private readonly IPagamentoBll _pagamentoBll;
        public EditModel(IPagamentoBll pagamentoBll)
        {
            _pagamentoBll = pagamentoBll;
        }

        [BindProperty]
        public PagamentoVM Input { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var vm = await _pagamentoBll.ObterPorIdAsync(id);
            if (vm == null)
                return NotFound();
            Input = vm;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            try
            {
                await _pagamentoBll.AtualizarAsync(Input);
                TempData["Success"] = "Pagamento atualizado.";
                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }
    }
}
