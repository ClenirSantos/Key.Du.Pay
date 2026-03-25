using Key.Du.Pay.BusinessLogic.Cobranca;
using Key.Du.Pay.BusinessLogic.PlanoPagamento;
using Key.Du.Pay.CrossCutting.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Key.Du.Pay.UserInterface.Pages.Cobranca
{
    public class EditModel : PageModel
    {
        private readonly ICobrancaBll _cobrancaBll;
        private readonly IPlanoPagamentoBll _planoBll;

        public EditModel(ICobrancaBll cobrancaBll, IPlanoPagamentoBll planoBll)
        {
            _cobrancaBll = cobrancaBll;
            _planoBll = planoBll;
        }

        [BindProperty]
        public CobrancaVM Input { get; set; } = new() { Descricao = "" };

        public SelectList? Planos { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var planos = await _planoBll.ListarAsync();
            Planos = new SelectList(planos, "Id", "Id");
            var vm = await _cobrancaBll.ObterPorIdAsync(id);
            if (vm == null)
                return NotFound();
            Input = vm;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var planos = await _planoBll.ListarAsync();
            Planos = new SelectList(planos, "Id", "Id");

            if (!ModelState.IsValid)
                return Page();

            try
            {
                await _cobrancaBll.AtualizarAsync(Input);
                TempData["Success"] = "Cobrança atualizada.";
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
