
namespace Ev.Service.Contacts.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// GenericRepository class.
    /// </summary>
    /// <typeparam name="TEntity">entity type.</typeparam>
    public class GenericRepository<TEntity> : IGenericRepository<TEntity>
         where TEntity : class
    {
        private readonly ContactDBContext context;
        private readonly DbSet<TEntity> dbSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericRepository{TEntity}"/> class.
        /// </summary>
        public GenericRepository()
            : this(new ContactDBContext())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericRepository{TEntity}" /> class.
        /// </summary>
        /// <param name="context">dbcontext.</param>
        public GenericRepository(ContactDBContext context)
        {
            this.context = context;
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            this.dbSet = this.context.Set<TEntity>();
        }

        /// <summary>
        /// Insert.
        /// </summary>
        /// <param name="entity">entity.</param>
        public virtual void Insert(TEntity entity)
        {
            this.dbSet.Add(entity);
        }

        /// <summary>
        /// Update.
        /// </summary>
        /// <param name="entityToUpdate">entityToUpdate.</param>
        public virtual void Update(TEntity entityToUpdate)
        {
            this.dbSet.Attach(entityToUpdate);
            this.context.Entry(entityToUpdate).State = EntityState.Modified;
        }

        /// <summary>
        /// Gets the list of entities.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="includeProperties">The include properties.</param>
        /// <returns>List of entities.</returns>
        public async Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter = null, int? limit = null, int? offset = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "")
        {
            IQueryable<TEntity> query = this.dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (offset.HasValue)
            {
                query = query.Skip(offset.Value);
            }

            if (limit.HasValue)
            {
                query = query.Take(limit.Value);
            }

            if (includeProperties != null)
            {
                foreach (var includeProperty in includeProperties.Split(
                new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            return await query.ToListAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Get All.
        /// </summary>
        /// <returns>Ienumerable entity.</returns>
        public IEnumerable<TEntity> GetAll()
        {
            return this.dbSet;
        }

        /// <summary>
        /// Delete.
        /// </summary>
        /// <param name="id">object.</param>
        /// <returns>entity.</returns>
        public async Task<TEntity> GetByIdAsync(object id)
        {
            return await this.dbSet.FindAsync(id).ConfigureAwait(false);
        }

        /// <summary>
        /// Save whaterv in context.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task SaveAsync()
        {
            await this.context.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Get total record count.
        /// </summary>
        /// <returns>record count.</returns>
        public int GetTotalCount()
        {
            IQueryable<TEntity> query = this.dbSet;

            return query.Count();
        }
    }
}
