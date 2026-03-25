using Key.Du.Pay.BusinessLogic.ResponsavelFinanceiro;
using Key.Du.Pay.CrossCutting.Enum;
using Key.Du.Pay.CrossCutting.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Key.Du.Pay.UserInterface.Pages.ResponsavelFinanceiro
{
    public class CreateModel : PageModel
    {
        private readonly IResponsavelFinanceiroBll _bll;

        public CreateModel(IResponsavelFinanceiroBll bll)
        {
            _bll = bll;
        }

        [BindProperty]
        public ResponsavelFinanceiroVM Input { get; set; } = new()
        {
            Descricao = "",
            Adimplente = EnumAdimplencia.Adimplente,
            TipoUsuario = EnumTipoUsuario.ResponsavelFinanceiro
        };

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
                TempData["Success"] = "Responsável cadastrado com sucesso.";
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
