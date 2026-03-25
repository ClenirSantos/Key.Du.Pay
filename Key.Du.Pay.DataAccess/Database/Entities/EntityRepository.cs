using Key.Du.Pay.CrossCutting.Filters;
using Key.Du.Pay.DataAccess.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Key.Du.Pay.DataAccess.Database.Entities
{
    public class EntityRepository<T> : IRepository<T> where T : class
    {
        public AppDbContext Context { get; private set; }

        protected DbSet<T> Set => Context.Set<T>();

        public EntityRepository(AppDbContext db_dbContext)
        {
            Context = db_dbContext;
        }

        public IQueryable<T> Query => Set;

        public Task<List<T>> FindAllAsync()
        {
            return Set.AsNoTracking().ToListAsync();
        }

        public async Task AdicionarAsync(T entity)
        {
            var entry = Context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                await Set.AddAsync(entity);
            }
        }

        public Task SalvarAsync(T entity)
        {
            var props = typeof(T)
                .GetProperties()
                .Where(prop =>
                    Attribute.IsDefined(prop,
                        typeof(System.ComponentModel.DataAnnotations.KeyAttribute)));

            var codeValue = props?.First()?.GetValue(entity)?.GetType().Name == "Int64" ? (long?)props?.First()?.GetValue(entity) : (int?)props?.First()?.GetValue(entity);

            if (codeValue == 0)
            {
                return this.AdicionarAsync(entity);
            }

            this.Update(entity);
            return Task.CompletedTask;
        }

        public Task InserirAsync(T entity)
        {
            var props = typeof(T)
                .GetProperties()
                .Where(prop =>
                    Attribute.IsDefined(prop,
                        typeof(System.ComponentModel.DataAnnotations.KeyAttribute)));

            if (props.Any())
            {
                var codeValue = props?.First()?.GetValue(entity)?.GetType().Name == "Int64" ? (long?)props?.First()?.GetValue(entity) : (int?)props?.First()?.GetValue(entity);
            }

            return this.AdicionarAsync(entity);
        }

        public void Apagar(T entity)
        {
            var entry = Context.Entry(entity);
            if (entry.State == EntityState.Detached)
                Set.Attach(entity);

            Set.Remove(entity);
        }

        public void Update(T entity)
        {
            var entry = Context.Entry(entity);
            if (entry.State == EntityState.Detached)
                Set.Attach(entity);

            entry.State = EntityState.Modified;
        }

        public Task<T> SelecionarAsync(Expression<Func<T, bool>> predicate)
        {
            return Context
                    .Set<T>()
                    .AsNoTracking()
                    .SingleOrDefaultAsync(predicate);
        }


        public Task<T> SelecionarUnicoAsync(Expression<Func<T, bool>> predicate)
        {
            return Context
                   .Set<T>()
                   .AsNoTracking()
                   .SingleAsync(predicate);
        }

        public Task<List<T>> ListarAsync(Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string includeProperties = "")
        {
            IQueryable<T> query = Context.Set<T>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query)
                    .AsNoTracking()
                    .ToListAsync();
            }

            return query
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<PagedItems<T>> ListarPaginadoAsync(Expression<Func<T, bool>>? predicate,
            PagedOptions pagedFilter)
        {
            if (string.IsNullOrEmpty(pagedFilter.Sort) && pagedFilter.SortManny == null)
            {
                var props = typeof(T)
                    .GetProperties()
                    .Where(prop =>
                        Attribute.IsDefined(prop,
                            typeof(System.ComponentModel.DataAnnotations.KeyAttribute)));

                pagedFilter.Sort = props?.First().Name;
            }

            PagedItems<T> paged = new PagedItems<T>();

            var query = Context
                .Set<T>()
                .AsNoTracking()
                .AsQueryable();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            paged.Total = await query.CountAsync();

            if (!string.IsNullOrEmpty(pagedFilter.Sort))
            {
                query = LinqExtension.OrderBy(query, pagedFilter.Sort, pagedFilter.Reverse);
            }
            else
            {
                if (pagedFilter.SortManny != null)
                {
                    var list = pagedFilter.SortManny.ToList();
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (i == 0)
                        {
                            query = LinqExtension.OrderBy(query, list[i].Sort, list[i].Reverse);
                        }
                        else
                        {
                            query = LinqExtension.ThenBy(query, list[i].Sort, list[i].Reverse);
                        }
                    }
                }
            }

            var skip = (pagedFilter.Page.Value * pagedFilter.Size.Value) - pagedFilter.Size.Value;
            query = query.Skip(skip);
            query = query.Take(pagedFilter.Size.Value);

            paged.Items = await query.ToListAsync();
            return paged;
        }

        public Task<bool> ExisteAsync(Expression<Func<T, bool>> predicate)
        {
            return Context
                .Set<T>()
                .AsNoTracking()
                .AnyAsync(predicate);
        }

        public void DetachEntries()
        {
            foreach (var entry in this.Context.ChangeTracker.Entries())
            {
                entry.State = EntityState.Detached;
            }
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return Context.SaveChangesAsync(cancellationToken);
        }
    }
}
