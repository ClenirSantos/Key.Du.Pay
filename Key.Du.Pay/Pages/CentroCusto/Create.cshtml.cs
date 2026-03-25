using Key.Du.Pay.BusinessLogic.CentroCusto;
using Key.Du.Pay.CrossCutting.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Key.Du.Pay.UserInterface.Pages.CentroCusto
{
    public class CreateModel : PageModel
    {
        private readonly ICentroCustoBll _bll;

        public CreateModel(ICentroCustoBll bll)
        {
            _bll = bll;
        }

        [BindProperty]
        public CentroCustoVM Input { get; set; } = new() { Descricao = "" };

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            try
            {
                await _bll.CriarAsync(Input);
                TempData["Success"] = "Centro de custo cadastrado.";
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
