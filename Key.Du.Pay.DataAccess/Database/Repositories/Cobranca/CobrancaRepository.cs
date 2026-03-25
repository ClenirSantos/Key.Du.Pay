using Key.Du.Pay.DataAccess.Database.Entities;

namespace Key.Du.Pay.DataAccess.Database.Repositories.Cobranca
{
    public class CobrancaRepository : EntityRepository<CobrancaEntity>, ICobrancaRepository
    {
        public CobrancaRepository(AppDbContext db_dbContext) : base(db_dbContext)
        {
        }
    }
}
