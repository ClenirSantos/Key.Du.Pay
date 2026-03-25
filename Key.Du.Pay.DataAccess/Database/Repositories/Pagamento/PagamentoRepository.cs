using Key.Du.Pay.DataAccess.Database.Entities;

namespace Key.Du.Pay.DataAccess.Database.Repositories.Pagamento
{
    public class PagamentoRepository : EntityRepository<PagamentoEntity>, IPagamentoRepository
    {
        public PagamentoRepository(AppDbContext db_dbContext) : base(db_dbContext)
        {
        }
    }
}
