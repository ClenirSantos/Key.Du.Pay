using Key.Du.Pay.CrossCutting.ViewModel;
using Key.Du.Pay.DataAccess.Database.Entities;
using Key.Du.Pay.DataAccess.Database.Repositories;
using Key.Du.Pay.DataAccess.Database.Repositories.CentroCusto;
using Key.Du.Pay.DataAccess.Database.Repositories.PlanoPagamento;

namespace Key.Du.Pay.BusinessLogic.CentroCusto
{
    public class CentroCustoBll : ICentroCustoBll
    {
        private readonly IRepository<CentroCustoEntity> _centroCustoRepository;
        private readonly IPlanoPagamentoRepository _planoPagamentoRepository;

        public CentroCustoBll(
            ICentroCustoRepository centroCustoRepository,
            IPlanoPagamentoRepository planoPagamentoRepository)
        {
            _centroCustoRepository = centroCustoRepository;
            _planoPagamentoRepository = planoPagamentoRepository;
        }

        public async Task<List<CentroCustoVM>> ListarAsync()
        {
            var entities = await _centroCustoRepository.ListarAsync();
            return entities.Select(e => MapToVM(e)).ToList();
        }

        public async Task<CentroCustoVM?> ObterPorIdAsync(int id)
        {
            var entity = await _centroCustoRepository.SelecionarAsync(x => x.Id == id);
            return entity == null ? null : MapToVM(entity);
        }

        public async Task<CentroCustoVM> CriarAsync(CentroCustoVM entrada)
        {
            var entity = new CentroCustoEntity { Descricao = entrada.Descricao };
            await _centroCustoRepository.SalvarAsync(entity);
            await _centroCustoRepository.SaveChangesAsync();
            return MapToVM(entity);
        }

        public async Task AtualizarAsync(CentroCustoVM entrada)
        {
            var atual = await _centroCustoRepository.SelecionarAsync(x => x.Id == entrada.Id)
                ?? throw new InvalidOperationException("Centro de custo não encontrado.");
            atual.Descricao = entrada.Descricao;
            _centroCustoRepository.Update(atual);
            await _centroCustoRepository.SaveChangesAsync();
        }

        public async Task ExcluirAsync(int id)
        {
            if (await _planoPagamentoRepository.ExisteAsync(p => p.CentroCustoID == id))
                throw new InvalidOperationException("Existem planos de pagamento vinculados a este centro de custo.");

            var atual = await _centroCustoRepository.SelecionarAsync(x => x.Id == id)
                ?? throw new InvalidOperationException("Centro de custo não encontrado.");
            _centroCustoRepository.Apagar(atual);
            await _centroCustoRepository.SaveChangesAsync();
        }

        private static CentroCustoVM MapToVM(CentroCustoEntity item) => new()
        {
            Id = item.Id,
            Descricao = item.Descricao
        };
    }
}
