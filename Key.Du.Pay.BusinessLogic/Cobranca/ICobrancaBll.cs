using Key.Du.Pay.CrossCutting.ViewModel;

namespace Key.Du.Pay.BusinessLogic.Cobranca
{
    public interface ICobrancaBll
    {
        Task<List<CobrancaVM>> ListarAsync();

        Task<CobrancaVM?> ObterPorIdAsync(int id);

        Task<CobrancaVM> CriarAsync(CobrancaVM entrada);

        Task AtualizarAsync(CobrancaVM entrada);

        Task ExcluirAsync(int id);

        Task<List<CobrancaVM>> ListarPorResponsavelAsync(int responsavelId);

        Task<int> QuantidadePorResponsavelAsync(int responsavelId);
    }
}
