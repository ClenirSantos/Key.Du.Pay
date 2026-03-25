using Key.Du.Pay.DataAccess.Database.Entities;

namespace Key.Du.Pay.DataAccess.Database.Repositories.PlanoPagamento
{
    public class PlanoPagamentoRepository : EntityRepository<PlanoPagamentoEntity>, IPlanoPagamentoRepository
    {
        public PlanoPagamentoRepository(AppDbContext db_dbContext) : base(db_dbContext)
        {
        }
    }
}
