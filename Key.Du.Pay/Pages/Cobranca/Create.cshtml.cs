using Key.Du.Pay.BusinessLogic.Cobranca;
using Key.Du.Pay.BusinessLogic.PlanoPagamento;
using Key.Du.Pay.CrossCutting.Enum;
using Key.Du.Pay.CrossCutting.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Key.Du.Pay.UserInterface.Pages.Cobranca
{
    public class CreateModel : PageModel
    {
        private readonly ICobrancaBll _cobrancaBll;
        private readonly IPlanoPagamentoBll _planoBll;

        public CreateModel(ICobrancaBll cobrancaBll, IPlanoPagamentoBll planoBll)
        {
            _cobrancaBll = cobrancaBll;
            _planoBll = planoBll;
        }

        [BindProperty]
        public CobrancaVM Input { get; set; } = new() { Descricao = "" };

        public SelectList? Planos { get; set; }

        public async Task OnGetAsync()
        {
            var planos = await _planoBll.ListarAsync();
            Planos = new SelectList(planos, "Id", "Id");
            Input = new CobrancaVM
            {
                Descricao = "",
                DataVencimento = DateTime.UtcNow.Date.AddMonths(1),
                MetodoPagamento = EnumMetodoPagamento.Boleto,
                Valor = 0,
                Status = EnumCobrancaStatus.Emitida
            };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var planos = await _planoBll.ListarAsync();
            Planos = new SelectList(planos, "Id", "Id");

            if (!ModelState.IsValid)
                return Page();

            try
            {
                await _cobrancaBll.CriarAsync(Input);
                TempData["Success"] = "Cobrança criada.";
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
