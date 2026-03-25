using Key.Du.Pay.CrossCutting.Enum;
using Key.Du.Pay.CrossCutting.ViewModel;
using Key.Du.Pay.DataAccess.Database.Entities;
using Key.Du.Pay.DataAccess.Database.Repositories.Cobranca;
using Key.Du.Pay.DataAccess.Database.Repositories.PlanoPagamento;
using Microsoft.EntityFrameworkCore;

namespace Key.Du.Pay.BusinessLogic.Cobranca
{
    public class CobrancaBll : ICobrancaBll
    {
        private readonly ICobrancaRepository _cobrancaRepository;
        private readonly IPlanoPagamentoRepository _planoPagamentoRepository;

        public CobrancaBll(
            ICobrancaRepository cobrancaRepository,
            IPlanoPagamentoRepository planoPagamentoRepository)
        {
            _cobrancaRepository = cobrancaRepository;
            _planoPagamentoRepository = planoPagamentoRepository;
        }

        public async Task<List<CobrancaVM>> ListarAsync()
        {
            var entities = await _cobrancaRepository.ListarAsync(
                null,
                null,
                "PlanoPagamento,PlanoPagamento.CentroCusto,PlanoPagamento.ResponsavelFinanceiro");
            return entities.Select(MapToVM).ToList();
        }

        public async Task<CobrancaVM?> ObterPorIdAsync(int id)
        {
            var list = await _cobrancaRepository.ListarAsync(
                c => c.Id == id,
                null,
                "PlanoPagamento,PlanoPagamento.CentroCusto,PlanoPagamento.ResponsavelFinanceiro");
            var entity = list.FirstOrDefault();
            return entity == null ? null : MapToVM(entity);
        }

        public async Task<CobrancaVM> CriarAsync(CobrancaVM entrada)
        {
            var planoNav = await _planoPagamentoRepository.Context.Set<PlanoPagamentoEntity>()
                .Include(p => p.CentroCusto)
                .Include(p => p.ResponsavelFinanceiro)
                .FirstOrDefaultAsync(p => p.Id == entrada.PlanoPagamentoID)
                ?? throw new InvalidOperationException("Plano de pagamento não encontrado.");

            var venc = entrada.DataVencimento.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(entrada.DataVencimento, DateTimeKind.Utc)
                : entrada.DataVencimento.ToUniversalTime();

            var entity = new CobrancaEntity
            {
                Descricao = entrada.Descricao,
                DataVencimento = venc,
                MetodoPagamento = entrada.MetodoPagamento,
                Valor = entrada.Valor,
                Status = EnumCobrancaStatus.Emitida,
                CodigoPagamento = CobrancaCodigoGerador.Gerar(entrada.MetodoPagamento),
                PlanoPagamento = planoNav,
                PlanoPagamentoID = planoNav.Id
            };

            await _cobrancaRepository.SalvarAsync(entity);
            await _cobrancaRepository.SaveChangesAsync();

            return (await ObterPorIdAsync(entity.Id))!;
        }

        public async Task AtualizarAsync(CobrancaVM entrada)
        {
            var tracked = await _cobrancaRepository.Context.Set<CobrancaEntity>()
                .Include(c => c.PlanoPagamento)
                .FirstOrDefaultAsync(c => c.Id == entrada.Id)
                ?? throw new InvalidOperationException("Cobrança não encontrada.");

            var metodoAnterior = tracked.MetodoPagamento;

            if (entrada.PlanoPagamentoID != tracked.PlanoPagamentoID)
            {
                var novoPlano = await _planoPagamentoRepository.Context.Set<PlanoPagamentoEntity>()
                    .Include(p => p.CentroCusto)
                    .Include(p => p.ResponsavelFinanceiro)
                    .FirstOrDefaultAsync(p => p.Id == entrada.PlanoPagamentoID)
                    ?? throw new InvalidOperationException("Plano de pagamento não encontrado.");
                tracked.PlanoPagamento = novoPlano;
                tracked.PlanoPagamentoID = novoPlano.Id;
            }

            tracked.Descricao = entrada.Descricao;
            tracked.DataVencimento = entrada.DataVencimento.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(entrada.DataVencimento, DateTimeKind.Utc)
                : entrada.DataVencimento.ToUniversalTime();
            tracked.MetodoPagamento = entrada.MetodoPagamento;
            tracked.Valor = entrada.Valor;
            tracked.Status = entrada.Status;

            if (entrada.MetodoPagamento != metodoAnterior)
                tracked.CodigoPagamento = CobrancaCodigoGerador.Gerar(entrada.MetodoPagamento);

            await _cobrancaRepository.SaveChangesAsync();
        }

        public async Task ExcluirAsync(int id)
        {
            var atual = await _cobrancaRepository.SelecionarAsync(c => c.Id == id)
                ?? throw new InvalidOperationException("Cobrança não encontrada.");
            _cobrancaRepository.Apagar(atual);
            await _cobrancaRepository.SaveChangesAsync();
        }

        public async Task<List<CobrancaVM>> ListarPorResponsavelAsync(int responsavelId)
        {
            var entities = await _cobrancaRepository.ListarAsync(
                c => c.PlanoPagamento.ResponsavelFinanceiroID == responsavelId,
                null,
                "PlanoPagamento,PlanoPagamento.CentroCusto,PlanoPagamento.ResponsavelFinanceiro");
            return entities.Select(MapToVM).ToList();
        }

        public Task<int> QuantidadePorResponsavelAsync(int responsavelId)
        {
            return _cobrancaRepository.Query.CountAsync(c =>
                c.PlanoPagamento.ResponsavelFinanceiroID == responsavelId);
        }

        private static CobrancaVM MapToVM(CobrancaEntity c)
        {
            var plano = c.PlanoPagamento;
            return new CobrancaVM
            {
                Id = c.Id,
                Descricao = c.Descricao,
                DataVencimento = c.DataVencimento,
                MetodoPagamento = c.MetodoPagamento,
                Valor = c.Valor,
                Status = c.Status,
                CodigoPagamento = c.CodigoPagamento,
                PlanoPagamentoID = c.PlanoPagamentoID,
                PlanoPagamento = new PlanoPagamentoVM
                {
                    Id = plano.Id,
                    CentroCustoID = plano.CentroCustoID,
                    ResponsavelFinanceiroID = plano.ResponsavelFinanceiroID,
                    ValorTotal = 0,
                    CentroCusto = new CentroCustoVM
                    {
                        Id = plano.CentroCusto.Id,
                        Descricao = plano.CentroCusto.Descricao
                    },
                    ResponsavelFinanceiro = new ResponsavelFinanceiroVM
                    {
                        Id = plano.ResponsavelFinanceiro.Id,
                        Descricao = plano.ResponsavelFinanceiro.Descricao,
                        DataCadastro = plano.ResponsavelFinanceiro.DataCadastro,
                        Adimplente = plano.ResponsavelFinanceiro.Adimplente,
                        TipoUsuario = plano.ResponsavelFinanceiro.TipoUsuario
                    },
                    CobrancaEntities = null
                }
            };
        }
    }
}
