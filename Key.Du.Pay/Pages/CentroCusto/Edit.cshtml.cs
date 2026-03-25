using Key.Du.Pay.BusinessLogic.CentroCusto;
using Key.Du.Pay.CrossCutting.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Key.Du.Pay.UserInterface.Pages.CentroCusto
{
    public class EditModel : PageModel
    {
        private readonly ICentroCustoBll _bll;

        public EditModel(ICentroCustoBll bll)
        {
            _bll = bll;
        }

        [BindProperty]
        public CentroCustoVM Input { get; set; } = new() { Descricao = "" };

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var vm = await _bll.ObterPorIdAsync(id);
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
                await _bll.AtualizarAsync(Input);
                TempData["Success"] = "Centro de custo atualizado.";
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
