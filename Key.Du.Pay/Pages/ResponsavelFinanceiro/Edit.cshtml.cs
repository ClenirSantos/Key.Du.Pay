using Key.Du.Pay.BusinessLogic.ResponsavelFinanceiro;
using Key.Du.Pay.CrossCutting.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Key.Du.Pay.UserInterface.Pages.ResponsavelFinanceiro
{
    public class EditModel : PageModel
    {
        private readonly IResponsavelFinanceiroBll _bll;

        public EditModel(IResponsavelFinanceiroBll bll)
        {
            _bll = bll;
        }

        [BindProperty]
        public ResponsavelFinanceiroVM Input { get; set; } = new() { Descricao = "" };

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
                TempData["Success"] = "Responsável atualizado.";
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
