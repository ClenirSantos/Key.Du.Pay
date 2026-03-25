using Key.Du.Pay.CrossCutting.Enum;
using Key.Du.Pay.CrossCutting.ViewModel;
using Key.Du.Pay.DataAccess.Database.Entities;
using Key.Du.Pay.DataAccess.Database.Repositories.Cobranca;
using Key.Du.Pay.DataAccess.Database.Repositories.Pagamento;
using Microsoft.EntityFrameworkCore;

namespace Key.Du.Pay.BusinessLogic.Pagamento
{
    public class PagamentoBll : IPagamentoBll
    {
        private const decimal ToleranciaQuitacao = 0.01m;

        private readonly IPagamentoRepository _pagamentoRepository;
        private readonly ICobrancaRepository _cobrancaRepository;

        public PagamentoBll(
            IPagamentoRepository pagamentoRepository,
            ICobrancaRepository cobrancaRepository)
        {
            _pagamentoRepository = pagamentoRepository;
            _cobrancaRepository = cobrancaRepository;
        }

        public async Task<List<PagamentoVM>> ListarAsync()
        {
            var entities = await _pagamentoRepository.ListarAsync(null, null, "Cobranca");
            return entities.Select(MapToVM).ToList();
        }

        public async Task<PagamentoVM?> ObterPorIdAsync(int id)
        {
            var list = await _pagamentoRepository.ListarAsync(p => p.Id == id, null, "Cobranca");
            var entity = list.FirstOrDefault();
            return entity == null ? null : MapToVM(entity);
        }

        public async Task<List<PagamentoVM>> ListarPorCobrancaAsync(int cobrancaId)
        {
            var list = await _pagamentoRepository.ListarAsync(
                p => p.CobrancaID == cobrancaId,
                q => q.OrderByDescending(x => x.DataPagamento),
                "Cobranca"
            );
            return list.Select(MapToVM).ToList();
        }

        public async Task<decimal> ObterSaldoDevedorAsync(int cobrancaId)
        {
            var cobranca = await _cobrancaRepository.SelecionarAsync(c => c.Id == cobrancaId)
                ?? throw new InvalidOperationException("Cobrança não encontrada.");

            var totalPago = await _pagamentoRepository.Context.Set<PagamentoEntity>()
                .AsNoTracking()
                .Where(p => p.CobrancaID == cobrancaId)
                .SumAsync(p => (decimal?)p.Valor) ?? 0;

            return Math.Max(0, cobranca.Valor - totalPago);
        }

        public async Task<PagamentoVM> RegistrarAsync(PagamentoVM entrada)
        {
            var cobranca = await _cobrancaRepository.Context.Set<CobrancaEntity>()
                .Include(c => c.PlanoPagamento)
                    .ThenInclude(p => p.ResponsavelFinanceiro)
                .FirstOrDefaultAsync(c => c.Id == entrada.CobrancaID)
                ?? throw new InvalidOperationException("Cobrança não encontrada.");

            if (cobranca.Status == EnumCobrancaStatus.Cancelada)
                throw new InvalidOperationException("Não é permitido registrar pagamento em cobrança cancelada.");

            if (cobranca.Status == EnumCobrancaStatus.Paga)
                throw new InvalidOperationException("Cobrança já quitada.");

            var totalPago = await _pagamentoRepository.Context.Set<PagamentoEntity>()
                .Where(p => p.CobrancaID == cobranca.Id)
                .SumAsync(p => (decimal?)p.Valor) ?? 0;

            var saldo = cobranca.Valor - totalPago;
            if (saldo <= 0)
                throw new InvalidOperationException("Não há saldo a pagar nesta cobrança.");

            if (entrada.Valor <= 0)
                throw new InvalidOperationException("Informe um valor de pagamento maior que zero.");

            if (entrada.Valor > saldo)
                throw new InvalidOperationException($"O valor excede o saldo devedor ({saldo:N2}).");

            var novoTotalPago = totalPago + entrada.Valor;
            var responsavel = cobranca.PlanoPagamento.ResponsavelFinanceiro;

            if (novoTotalPago >= cobranca.Valor - ToleranciaQuitacao)
            {
                cobranca.Status = EnumCobrancaStatus.Paga;
                if (responsavel.Adimplente == EnumAdimplencia.RiscoInadimplencia)
                    responsavel.Adimplente = EnumAdimplencia.Adimplente;
            }
            else
            {
                cobranca.Status = EnumCobrancaStatus.PagaParcialmente;
                if (responsavel.Adimplente != EnumAdimplencia.Inadimplente)
                    responsavel.Adimplente = EnumAdimplencia.RiscoInadimplencia;
            }

            var dataPag = entrada.DataPagamento.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(entrada.DataPagamento, DateTimeKind.Utc)
                : entrada.DataPagamento.ToUniversalTime();

            var pagamento = new PagamentoEntity
            {
                Cobranca = cobranca,
                CobrancaID = cobranca.Id,
                DataPagamento = dataPag,
                Valor = entrada.Valor
            };

            await _pagamentoRepository.SalvarAsync(pagamento);
            await _pagamentoRepository.SaveChangesAsync();

            return (await ObterPorIdAsync(pagamento.Id))!;
        }

        public async Task AtualizarAsync(PagamentoVM entrada)
        {
            var tracked = await _pagamentoRepository.Context.Set<PagamentoEntity>()
                .Include(p => p.Cobranca)
                .FirstOrDefaultAsync(p => p.Id == entrada.Id)
                ?? throw new InvalidOperationException("Pagamento não encontrado.");

            tracked.DataPagamento = entrada.DataPagamento.Kind == DateTimeKind.Unspecified
                ? DateTime.SpecifyKind(entrada.DataPagamento, DateTimeKind.Utc)
                : entrada.DataPagamento.ToUniversalTime();
            tracked.Valor = entrada.Valor;

            await _pagamentoRepository.SaveChangesAsync();
        }

        public async Task ExcluirAsync(int id)
        {
            var atual = await _pagamentoRepository.SelecionarAsync(p => p.Id == id)
                ?? throw new InvalidOperationException("Pagamento não encontrado.");
            _pagamentoRepository.Apagar(atual);
            await _pagamentoRepository.SaveChangesAsync();
        }

        private static PagamentoVM MapToVM(PagamentoEntity p)
        {
            var c = p.Cobranca;
            return new PagamentoVM
            {
                Id = p.Id,
                CobrancaID = p.CobrancaID,
                DataPagamento = p.DataPagamento,
                Valor = p.Valor,
                Cobranca = new CobrancaVM
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
                }
            };
        }
    }
}
