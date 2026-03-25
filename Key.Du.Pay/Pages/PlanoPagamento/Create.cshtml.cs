using Key.Du.Pay.BusinessLogic.CentroCusto;
using Key.Du.Pay.BusinessLogic.PlanoPagamento;
using Key.Du.Pay.BusinessLogic.ResponsavelFinanceiro;
using Key.Du.Pay.CrossCutting.Enum;
using Key.Du.Pay.CrossCutting.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Key.Du.Pay.UserInterface.Pages.PlanoPagamento
{
    public class CreateModel : PageModel
    {
        private readonly IPlanoPagamentoBll _planoBll;
        private readonly IResponsavelFinanceiroBll _responsavelBll;
        private readonly ICentroCustoBll _centroBll;

        public CreateModel(
            IPlanoPagamentoBll planoBll,
            IResponsavelFinanceiroBll responsavelBll,
            ICentroCustoBll centroBll)
        {
            _planoBll = planoBll;
            _responsavelBll = responsavelBll;
            _centroBll = centroBll;
        }

        [BindProperty]
        public PlanoPagamentoCreateVM Input { get; set; } = new();

        public SelectList? Responsaveis { get; set; }
        public SelectList? Centros { get; set; }

        public async Task OnGetAsync()
        {
            await LoadLookupsAsync();
            if (Input.Cobrancas.Count == 0)
            {
                Input.Cobrancas = Enumerable.Range(0, 1).Select(i => new CobrancaPlanoItemVM
                {
                    Valor = 0,
                    DataVencimento = DateTime.UtcNow.Date.AddMonths(i + 1),
                    MetodoPagamento = i % 2 == 0 ? EnumMetodoPagamento.Boleto : EnumMetodoPagamento.Pix
                }).ToList();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadLookupsAsync();
            var linhas = Input.Cobrancas.Where(c => c.Valor > 0).ToList();
            if (linhas.Count == 0)
                ModelState.AddModelError(string.Empty, "Informe ao menos uma cobrança com valor maior que zero.");

            if (!ModelState.IsValid)
                return Page();

            try
            {
                await _planoBll.CriarAsync(new PlanoPagamentoCreateVM
                {
                    ResponsavelId = Input.ResponsavelId,
                    CentroDeCusto = Input.CentroDeCusto,
                    Cobrancas = linhas
                });
                TempData["Success"] = "Plano de pagamento criado.";
                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }
        }

        private async Task LoadLookupsAsync()
        {
            var r = await _responsavelBll.ListarAsync();
            var c = await _centroBll.ListarAsync();
            Responsaveis = new SelectList(r, "Id", "Descricao");
            Centros = new SelectList(c, "Id", "Descricao");
        }
    }
}
