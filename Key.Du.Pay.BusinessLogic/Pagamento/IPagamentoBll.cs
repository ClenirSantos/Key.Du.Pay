using Key.Du.Pay.CrossCutting.ViewModel;

namespace Key.Du.Pay.BusinessLogic.Pagamento
{
    public interface IPagamentoBll
    {
        Task<List<PagamentoVM>> ListarAsync();

        Task<PagamentoVM?> ObterPorIdAsync(int id);

        Task<List<PagamentoVM>> ListarPorCobrancaAsync(int cobrancaId);

        Task<decimal> ObterSaldoDevedorAsync(int cobrancaId);

        Task<PagamentoVM> RegistrarAsync(PagamentoVM entrada);

        Task AtualizarAsync(PagamentoVM entrada);

        Task ExcluirAsync(int id);
    }
}
