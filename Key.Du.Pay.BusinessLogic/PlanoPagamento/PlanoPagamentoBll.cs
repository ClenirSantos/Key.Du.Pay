using Key.Du.Pay.CrossCutting.Enum;
using Key.Du.Pay.CrossCutting.ViewModel;
using Key.Du.Pay.BusinessLogic.Cobranca;
using Key.Du.Pay.DataAccess.Database.Entities;
using Key.Du.Pay.DataAccess.Database.Repositories.CentroCusto;
using Key.Du.Pay.DataAccess.Database.Repositories.PlanoPagamento;
using Key.Du.Pay.DataAccess.Database.Repositories.ResponsavelFinanceiro;
using Microsoft.EntityFrameworkCore;

namespace Key.Du.Pay.BusinessLogic.PlanoPagamento
{
    public class PlanoPagamentoBll : IPlanoPagamentoBll
    {
        private readonly IPlanoPagamentoRepository _planoPagamentoRepository;
        private readonly ICentroCustoRepository _centroCustoRepository;
        private readonly IResponsavelFinanceiroRepository _responsavelFinanceiroRepository;

        public PlanoPagamentoBll(
            IPlanoPagamentoRepository planoPagamentoRepository,
            ICentroCustoRepository centroCustoRepository,
            IResponsavelFinanceiroRepository responsavelFinanceiroRepository)
        {
            _planoPagamentoRepository = planoPagamentoRepository;
            _centroCustoRepository = centroCustoRepository;
            _responsavelFinanceiroRepository = responsavelFinanceiroRepository;
        }

        public async Task<List<PlanoPagamentoVM>> ListarAsync()
        {
            var entities = await _planoPagamentoRepository.ListarAsync(
                null,
                null,
                "CentroCusto,ResponsavelFinanceiro,CobrancaEntities");
            return entities.Select(MapToVM).ToList();
        }

        public async Task<PlanoPagamentoVM?> ObterPorIdAsync(int id)
        {
            var entity = await _planoPagamentoRepository.ListarAsync(
                p => p.Id == id,
                null,
                "CentroCusto,ResponsavelFinanceiro,CobrancaEntities");
            var plano = entity.FirstOrDefault();
            return plano == null ? null : MapToVM(plano);
        }

        public async Task<decimal> ObterValorTotalAsync(int planoId)
        {
            var plano = await ObterPorIdAsync(planoId)
                ?? throw new InvalidOperationException("Plano de pagamento não encontrado.");
            return plano.ValorTotal;
        }

        public async Task<List<PlanoPagamentoVM>> ListarPorResponsavelAsync(int responsavelId)
        {
            var entities = await _planoPagamentoRepository.ListarAsync(
                p => p.ResponsavelFinanceiroID == responsavelId,
                null,
                "CentroCusto,ResponsavelFinanceiro,CobrancaEntities");
            return entities.Select(MapToVM).ToList();
        }

        public async Task<PlanoPagamentoVM> CriarAsync(PlanoPagamentoCreateVM entrada)
        {
            if (entrada.Cobrancas == null || entrada.Cobrancas.Count == 0)
                throw new InvalidOperationException("Informe ao menos uma cobrança para o plano.");

            var centro = await _centroCustoRepository.Context.Set<CentroCustoEntity>()
                .FindAsync(new object[] { entrada.CentroDeCusto })
                ?? throw new InvalidOperationException("Centro de custo não encontrado.");

            var responsavel = await _responsavelFinanceiroRepository.Context.Set<ResponsavelFinanceiroEntity>()
                .FindAsync(new object[] { entrada.ResponsavelId })
                ?? throw new InvalidOperationException("Responsável financeiro não encontrado.");

            await using var tx = await _planoPagamentoRepository.Context.Database.BeginTransactionAsync();

            var plano = new PlanoPagamentoEntity
            {
                CentroCusto = centro,
                ResponsavelFinanceiro = responsavel,
                CentroCustoID = centro.Id,
                ResponsavelFinanceiroID = responsavel.Id
            };

            var i = 0;
            foreach (var cob in entrada.Cobrancas)
            {
                i++;
                var venc = cob.DataVencimento.Kind == DateTimeKind.Unspecified
                    ? DateTime.SpecifyKind(cob.DataVencimento, DateTimeKind.Utc)
                    : cob.DataVencimento.ToUniversalTime();

                plano.CobrancaEntities.Add(new CobrancaEntity
                {
                    Descricao = cob.Descricao ?? $"Cobrança {i}",
                    Valor = cob.Valor,
                    DataVencimento = venc,
                    MetodoPagamento = cob.MetodoPagamento,
                    Status = EnumCobrancaStatus.Emitida,
                    CodigoPagamento = CobrancaCodigoGerador.Gerar(cob.MetodoPagamento),
                    PlanoPagamento = plano
                });
            }

            await _planoPagamentoRepository.Context.Set<PlanoPagamentoEntity>().AddAsync(plano);
            await _planoPagamentoRepository.SaveChangesAsync();
            await tx.CommitAsync();

            return (await ObterPorIdAsync(plano.Id))!;
        }

        public async Task AtualizarAsync(PlanoPagamentoVM entrada)
        {
            var tracked = await _planoPagamentoRepository.Context.Set<PlanoPagamentoEntity>()
                .Include(p => p.CentroCusto)
                .Include(p => p.ResponsavelFinanceiro)
                .Include(p => p.CobrancaEntities)
                .FirstOrDefaultAsync(p => p.Id == entrada.Id)
                ?? throw new InvalidOperationException("Plano de pagamento não encontrado.");

            var centro = await _centroCustoRepository.Context.Set<CentroCustoEntity>()
                .FindAsync(new object[] { entrada.CentroCustoID })
                ?? throw new InvalidOperationException("Centro de custo não encontrado.");

            var responsavel = await _responsavelFinanceiroRepository.Context.Set<ResponsavelFinanceiroEntity>()
                .FindAsync(new object[] { entrada.ResponsavelFinanceiroID })
                ?? throw new InvalidOperationException("Responsável financeiro não encontrado.");

            tracked.CentroCustoID = centro.Id;
            tracked.ResponsavelFinanceiroID = responsavel.Id;
            tracked.CentroCusto = centro;
            tracked.ResponsavelFinanceiro = responsavel;

            if (entrada.CobrancaEntities != null)
            {
                var dictTracked = tracked.CobrancaEntities.ToDictionary(c => c.Id);
                int i = tracked.CobrancaEntities.Count;
                foreach (var inputCob in entrada.CobrancaEntities)
                {
                    if (inputCob.Id > 0 && dictTracked.TryGetValue(inputCob.Id, out var existingCob))
                    {
                        if (inputCob.Status == EnumCobrancaStatus.Cancelada && existingCob.Status != EnumCobrancaStatus.Cancelada)
                        {
                            existingCob.Status = EnumCobrancaStatus.Cancelada;
                        }
                    }
                    else if (inputCob.Id == 0 && inputCob.Valor > 0)
                    {
                        i++;
                        var venc = inputCob.DataVencimento.Kind == DateTimeKind.Unspecified
                            ? DateTime.SpecifyKind(inputCob.DataVencimento, DateTimeKind.Utc)
                            : inputCob.DataVencimento.ToUniversalTime();

                        tracked.CobrancaEntities.Add(new CobrancaEntity
                        {
                            Descricao = inputCob.Descricao ?? $"Cobrança Extra {i}",
                            Valor = inputCob.Valor,
                            DataVencimento = venc,
                            MetodoPagamento = inputCob.MetodoPagamento,
                            Status = EnumCobrancaStatus.Emitida,
                            CodigoPagamento = CobrancaCodigoGerador.Gerar(inputCob.MetodoPagamento),
                            PlanoPagamento = tracked
                        });
                    }
                }
            }

            await _planoPagamentoRepository.SaveChangesAsync();
        }

        public async Task ExcluirAsync(int id)
        {
            var atual = await _planoPagamentoRepository.SelecionarAsync(p => p.Id == id)
                ?? throw new InvalidOperationException("Plano de pagamento não encontrado.");
            _planoPagamentoRepository.Apagar(atual);
            await _planoPagamentoRepository.SaveChangesAsync();
        }

        private static PlanoPagamentoVM MapToVM(PlanoPagamentoEntity p)
        {
            var cobrancas = p.CobrancaEntities?.Select(c => MapCobranca(c)).ToList() ?? new List<CobrancaVM>();
            var total = cobrancas.Where(c => c.Status != EnumCobrancaStatus.Cancelada).Sum(c => c.Valor);
            var totalCancelado = cobrancas.Where(c => c.Status == EnumCobrancaStatus.Cancelada).Sum(c => c.Valor);

            return new PlanoPagamentoVM
            {
                Id = p.Id,
                CentroCustoID = p.CentroCustoID,
                ResponsavelFinanceiroID = p.ResponsavelFinanceiroID,
                ValorTotal = total,
                ValorTotalCancelado = totalCancelado,
                CentroCusto = new CentroCustoVM
                {
                    Id = p.CentroCusto.Id,
                    Descricao = p.CentroCusto.Descricao
                },
                ResponsavelFinanceiro = new ResponsavelFinanceiroVM
                {
                    Id = p.ResponsavelFinanceiro.Id,
                    Descricao = p.ResponsavelFinanceiro.Descricao,
                    DataCadastro = p.ResponsavelFinanceiro.DataCadastro,
                    Adimplente = p.ResponsavelFinanceiro.Adimplente,
                    TipoUsuario = p.ResponsavelFinanceiro.TipoUsuario
                },
                CobrancaEntities = cobrancas
            };
        }

        private static CobrancaVM MapCobranca(CobrancaEntity c) => new()
        {
            Id = c.Id,
            Descricao = c.Descricao,
            DataVencimento = c.DataVencimento,
            MetodoPagamento = c.MetodoPagamento,
            Valor = c.Valor,
            Status = c.Status,
            CodigoPagamento = c.CodigoPagamento,
            PlanoPagamentoID = c.PlanoPagamentoID,
            PlanoPagamento = null
        };
    }
}
