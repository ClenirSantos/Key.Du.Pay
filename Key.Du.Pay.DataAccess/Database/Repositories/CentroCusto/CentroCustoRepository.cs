using Key.Du.Pay.DataAccess.Database.Entities;

namespace Key.Du.Pay.DataAccess.Database.Repositories.CentroCusto
{
    public class CentroCustoRepository : EntityRepository<CentroCustoEntity>, ICentroCustoRepository
    {
        public CentroCustoRepository(AppDbContext context) : base(context)
        {

        }
    }
}
