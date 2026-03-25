using Key.Du.Pay.BusinessLogic.Cobranca;
using Key.Du.Pay.BusinessLogic.Pagamento;
using Key.Du.Pay.BusinessLogic.PlanoPagamento;
using Key.Du.Pay.BusinessLogic.ResponsavelFinanceiro;
using Key.Du.Pay.CrossCutting.Enum;
using Key.Du.Pay.CrossCutting.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Key.Du.Pay.UserInterface.Pages.Pagamento
{
    public class CreateModel : PageModel
    {
        private readonly IPagamentoBll _pagamentoBll;
        private readonly ICobrancaBll _cobrancaBll;
        private readonly IPlanoPagamentoBll _planoBll;
        private readonly IResponsavelFinanceiroBll _responsavelBll;

        public CreateModel(
            IPagamentoBll pagamentoBll,
            ICobrancaBll cobrancaBll,
            IPlanoPagamentoBll planoBll,
            IResponsavelFinanceiroBll responsavelBll)
        {
            _pagamentoBll = pagamentoBll;
            _cobrancaBll = cobrancaBll;
            _planoBll = planoBll;
            _responsavelBll = responsavelBll;
        }

        [BindProperty]
        public PagamentoVM Input { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public int ResponsavelId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PlanoId { get; set; }

        [BindProperty]
        public bool PagamentoIntegral { get; set; }

        public List<ResponsavelFinanceiroVM> Responsaveis { get; set; } = new();
        public List<PlanoPagamentoVM> Planos { get; set; } = new();
        public List<CobrancaOpcaoPagamento> Opcoes { get; set; } = new();

        public CobrancaVM? CobrancaDetalhe { get; set; }
        public List<PagamentoVM> HistoricoPagamentos { get; set; } = new();

        public async Task OnGetAsync(int? cobrancaId)
        {
            // Se viemos da tela de cobrança com um cobrancaId, pré-resolver responsável e plano
            if (cobrancaId.HasValue && cobrancaId.Value > 0)
            {
                var cob = await _cobrancaBll.ObterPorIdAsync(cobrancaId.Value);
                if (cob != null)
                {
                    PlanoId = cob.PlanoPagamentoID;
                    var plano = await _planoBll.ObterPorIdAsync(PlanoId);
                    if (plano != null)
                        ResponsavelId = plano.ResponsavelFinanceiroID;
                }

                Input = new PagamentoVM
                {
                    DataPagamento = DateTime.UtcNow,
                    Valor = 0,
                    CobrancaID = cobrancaId.Value
                };
            }
            else
            {
                Input = new PagamentoVM
                {
                    DataPagamento = DateTime.UtcNow,
                    Valor = 0
                };
            }

            await CarregarSelects();

            if (Input.CobrancaID > 0)
                await CarregarDetalhesAsync(Input.CobrancaID);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await CarregarSelects();

            if (Input.CobrancaID <= 0)
            {
                ModelState.AddModelError(string.Empty, "Selecione uma cobrança.");
                return Page();
            }

            if (PagamentoIntegral)
            {
                try
                {
                    Input.Valor = await _pagamentoBll.ObterSaldoDevedorAsync(Input.CobrancaID);
                }
                catch (InvalidOperationException ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    await CarregarDetalhesAsync(Input.CobrancaID);
                    return Page();
                }
            }

            if (!ModelState.IsValid)
            {
                if (Input.CobrancaID > 0)
                    await CarregarDetalhesAsync(Input.CobrancaID);
                return Page();
            }

            try
            {
                await _pagamentoBll.RegistrarAsync(Input);
                TempData["Success"] = "Pagamento registrado.";
                return RedirectToPage("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                if (Input.CobrancaID > 0)
                    await CarregarDetalhesAsync(Input.CobrancaID);
                return Page();
            }
        }

        private async Task CarregarDetalhesAsync(int cobrancaId)
        {
            CobrancaDetalhe = await _cobrancaBll.ObterPorIdAsync(cobrancaId);
            HistoricoPagamentos = await _pagamentoBll.ListarPorCobrancaAsync(cobrancaId);
        }

        private async Task CarregarSelects()
        {
            // 1. Responsáveis (sempre todos)
            Responsaveis = await _responsavelBll.ListarAsync();

            // 2. Planos do responsável selecionado
            if (ResponsavelId > 0)
                Planos = await _planoBll.ListarPorResponsavelAsync(ResponsavelId);

            // 3. Cobranças elegíveis do plano selecionado
            if (PlanoId > 0)
            {
                var todasCobranças = await _cobrancaBll.ListarAsync();
                var elegíveis = todasCobranças
                    .Where(c => c.PlanoPagamentoID == PlanoId
                             && c.Status != EnumCobrancaStatus.Paga
                             && c.Status != EnumCobrancaStatus.Cancelada)
                    .ToList();

                Opcoes = new List<CobrancaOpcaoPagamento>();
                foreach (var c in elegíveis)
                {
                    var saldo = await _pagamentoBll.ObterSaldoDevedorAsync(c.Id);
                    if (saldo <= 0) continue;

                    Opcoes.Add(new CobrancaOpcaoPagamento
                    {
                        Id = c.Id,
                        Saldo = saldo,
                        Texto = $"#{c.Id} — {c.Descricao} — saldo {saldo:N2}"
                    });
                }
            }
        }
    }
}
