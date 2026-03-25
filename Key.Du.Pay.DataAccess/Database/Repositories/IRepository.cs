using Key.Du.Pay.CrossCutting.Filters;
using System.Linq.Expressions;

namespace Key.Du.Pay.DataAccess.Database.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task SalvarAsync(T entity);
        Task InserirAsync(T entity);
        void Update(T entity);
        void Apagar(T entity);

        Task<List<T>> ListarAsync(Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string includeProperties = "");

        Task<T> SelecionarAsync(Expression<Func<T, bool>> predicate);

        Task<T> SelecionarUnicoAsync(Expression<Func<T, bool>> predicate);

        Task<PagedItems<T>> ListarPaginadoAsync(Expression<Func<T, bool>> predicate, PagedOptions pagedFilter);

        Task<List<T>> FindAllAsync();

        IQueryable<T> Query { get; }

        AppDbContext Context { get; }

        Task<bool> ExisteAsync(Expression<Func<T, bool>> predicate);

        void DetachEntries();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
