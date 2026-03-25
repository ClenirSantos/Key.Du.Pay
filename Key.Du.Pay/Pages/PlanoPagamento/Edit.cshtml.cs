using Key.Du.Pay.BusinessLogic.CentroCusto;
using Key.Du.Pay.BusinessLogic.PlanoPagamento;
using Key.Du.Pay.BusinessLogic.ResponsavelFinanceiro;
using Key.Du.Pay.CrossCutting.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Key.Du.Pay.UserInterface.Pages.PlanoPagamento
{
    public class EditModel : PageModel
    {
        private readonly IPlanoPagamentoBll _planoBll;
        private readonly IResponsavelFinanceiroBll _responsavelBll;
        private readonly ICentroCustoBll _centroBll;

        public EditModel(
            IPlanoPagamentoBll planoBll,
            IResponsavelFinanceiroBll responsavelBll,
            ICentroCustoBll centroBll)
        {
            _planoBll = planoBll;
            _responsavelBll = responsavelBll;
            _centroBll = centroBll;
        }

        public PlanoPagamentoVM? Plano { get; set; }

        [BindProperty]
        public PlanoPagamentoEditVM Input { get; set; } = new();

        public SelectList? Responsaveis { get; set; }
        public SelectList? Centros { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var vm = await _planoBll.ObterPorIdAsync(id);
            if (vm == null)
                return NotFound();
            Plano = vm;
            Input = new PlanoPagamentoEditVM
            {
                Id = vm.Id,
                ResponsavelFinanceiroID = vm.ResponsavelFinanceiroID,
                CentroCustoID = vm.CentroCustoID,
                Cobrancas = vm.CobrancaEntities?.Select(c => new CobrancaEditItemVM
                {
                    Id = c.Id,
                    Valor = c.Valor,
                    DataVencimento = c.DataVencimento,
                    MetodoPagamento = c.MetodoPagamento,
                    Descricao = c.Descricao,
                    Status = c.Status,
                    Cancelada = c.Status == Key.Du.Pay.CrossCutting.Enum.EnumCobrancaStatus.Cancelada
                }).ToList() ?? new List<CobrancaEditItemVM>()
            };
            await LoadLookupsAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadLookupsAsync();
            var existing = await _planoBll.ObterPorIdAsync(Input.Id);
            if (existing == null)
                return NotFound();

            Plano = existing;

            if (!ModelState.IsValid)
                return Page();

            try
            {
                existing.ResponsavelFinanceiroID = Input.ResponsavelFinanceiroID;
                existing.CentroCustoID = Input.CentroCustoID;
                existing.CobrancaEntities = Input.Cobrancas.Select(c => new CobrancaVM
                {
                    Id = c.Id,
                    Valor = c.Valor,
                    DataVencimento = c.DataVencimento,
                    MetodoPagamento = c.MetodoPagamento,
                    Descricao = c.Descricao,
                    Status = c.Cancelada ? Key.Du.Pay.CrossCutting.Enum.EnumCobrancaStatus.Cancelada : c.Status
                }).ToList();
                await _planoBll.AtualizarAsync(existing);
                TempData["Success"] = "Plano atualizado.";
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
