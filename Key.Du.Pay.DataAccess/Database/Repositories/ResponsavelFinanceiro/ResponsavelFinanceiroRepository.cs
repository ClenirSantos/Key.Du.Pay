using Key.Du.Pay.DataAccess.Database.Entities;

namespace Key.Du.Pay.DataAccess.Database.Repositories.ResponsavelFinanceiro
{
    public class ResponsavelFinanceiroRepository : EntityRepository<ResponsavelFinanceiroEntity>, IResponsavelFinanceiroRepository
    {
        public ResponsavelFinanceiroRepository(AppDbContext db_dbContext) : base(db_dbContext)
        {
        }
    }
}
