using Key.Du.Pay.CrossCutting.ViewModel;

namespace Key.Du.Pay.BusinessLogic.CentroCusto
{
    public interface ICentroCustoBll
    {
        Task<List<CentroCustoVM>> ListarAsync();

        Task<CentroCustoVM?> ObterPorIdAsync(int id);

        Task<CentroCustoVM> CriarAsync(CentroCustoVM entrada);

        Task AtualizarAsync(CentroCustoVM entrada);

        Task ExcluirAsync(int id);
    }
}
