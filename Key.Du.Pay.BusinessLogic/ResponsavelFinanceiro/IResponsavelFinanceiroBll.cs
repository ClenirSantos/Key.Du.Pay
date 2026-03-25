using Key.Du.Pay.CrossCutting.ViewModel;

namespace Key.Du.Pay.BusinessLogic.ResponsavelFinanceiro
{
    public interface IResponsavelFinanceiroBll
    {
        Task<List<ResponsavelFinanceiroVM>> ListarAsync();

        Task<ResponsavelFinanceiroVM?> ObterPorIdAsync(int id);

        Task<ResponsavelFinanceiroVM> CriarAsync(ResponsavelFinanceiroVM entrada);

        Task AtualizarAsync(ResponsavelFinanceiroVM entrada);

        Task ExcluirAsync(int id);
    }
}
