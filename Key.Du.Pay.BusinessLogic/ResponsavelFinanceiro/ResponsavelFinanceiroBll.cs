using Key.Du.Pay.CrossCutting.Enum;
using Key.Du.Pay.CrossCutting.ViewModel;
using Key.Du.Pay.DataAccess.Database.Entities;
using Key.Du.Pay.DataAccess.Database.Repositories;
using Key.Du.Pay.DataAccess.Database.Repositories.PlanoPagamento;
using Key.Du.Pay.DataAccess.Database.Repositories.ResponsavelFinanceiro;

namespace Key.Du.Pay.BusinessLogic.ResponsavelFinanceiro
{
    public class ResponsavelFinanceiroBll : IResponsavelFinanceiroBll
    {
        private readonly IRepository<ResponsavelFinanceiroEntity> _responsavelFinanceiroRepository;
        private readonly IPlanoPagamentoRepository _planoPagamentoRepository;

        public ResponsavelFinanceiroBll(
            IResponsavelFinanceiroRepository responsavelFinanceiroRepository,
            IPlanoPagamentoRepository planoPagamentoRepository)
        {
            _responsavelFinanceiroRepository = responsavelFinanceiroRepository;
            _planoPagamentoRepository = planoPagamentoRepository;
        }

        public async Task<List<ResponsavelFinanceiroVM>> ListarAsync()
        {
            var entities = await _responsavelFinanceiroRepository.ListarAsync();
            return entities.Select(MapToVM).ToList();
        }

        public async Task<ResponsavelFinanceiroVM?> ObterPorIdAsync(int id)
        {
            var entity = await _responsavelFinanceiroRepository.SelecionarAsync(x => x.Id == id);
            return entity == null ? null : MapToVM(entity);
        }

        public async Task<ResponsavelFinanceiroVM> CriarAsync(ResponsavelFinanceiroVM entrada)
        {
            var entity = new ResponsavelFinanceiroEntity
            {
                Descricao = entrada.Descricao,
                DataCadastro = DateTime.UtcNow,
                Adimplente = entrada.Adimplente,
                TipoUsuario = entrada.TipoUsuario
            };
            await _responsavelFinanceiroRepository.SalvarAsync(entity);
            await _responsavelFinanceiroRepository.SaveChangesAsync();
            return MapToVM(entity);
        }

        public async Task AtualizarAsync(ResponsavelFinanceiroVM entrada)
        {
            var atual = await _responsavelFinanceiroRepository.SelecionarAsync(x => x.Id == entrada.Id)
                ?? throw new InvalidOperationException("Responsável não encontrado.");
            atual.Descricao = entrada.Descricao;
            atual.Adimplente = entrada.Adimplente;
            atual.TipoUsuario = entrada.TipoUsuario;
            _responsavelFinanceiroRepository.Update(atual);
            await _responsavelFinanceiroRepository.SaveChangesAsync();
        }

        public async Task ExcluirAsync(int id)
        {
            if (await _planoPagamentoRepository.ExisteAsync(p => p.ResponsavelFinanceiroID == id))
                throw new InvalidOperationException("Existem planos de pagamento vinculados a este responsável.");

            var atual = await _responsavelFinanceiroRepository.SelecionarAsync(x => x.Id == id)
                ?? throw new InvalidOperationException("Responsável não encontrado.");
            _responsavelFinanceiroRepository.Apagar(atual);
            await _responsavelFinanceiroRepository.SaveChangesAsync();
        }

        private static ResponsavelFinanceiroVM MapToVM(ResponsavelFinanceiroEntity item) => new()
        {
            Id = item.Id,
            Descricao = item.Descricao,
            DataCadastro = item.DataCadastro,
            Adimplente = item.Adimplente,
            TipoUsuario = item.TipoUsuario
        };
    }
}
