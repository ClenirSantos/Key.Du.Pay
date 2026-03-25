using Key.Du.Pay.CrossCutting.ViewModel;

namespace Key.Du.Pay.BusinessLogic.PlanoPagamento
{
    public interface IPlanoPagamentoBll
    {
        Task<List<PlanoPagamentoVM>> ListarAsync();

        Task<PlanoPagamentoVM?> ObterPorIdAsync(int id);

        Task<decimal> ObterValorTotalAsync(int planoId);

        Task<List<PlanoPagamentoVM>> ListarPorResponsavelAsync(int responsavelId);

        Task<PlanoPagamentoVM> CriarAsync(PlanoPagamentoCreateVM entrada);

        Task AtualizarAsync(PlanoPagamentoVM entrada);

        Task ExcluirAsync(int id);
    }
}
