
namespace Ev.Service.Contacts.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    /// <summary>
    /// IGenericRepository.
    /// </summary>
    /// <typeparam name="TEntity"> generic entity.</typeparam>
    public interface IGenericRepository<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// Gets the list of entities.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="includeProperties">The include properties.</param>
        /// <returns>List of entities.</returns>
        Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>> filter = null, int? limit = null, int? offset = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, string includeProperties = "");

        /// <summary>
        /// Get All.
        /// </summary>
        /// <returns>Ienumerable entity.</returns>
        IEnumerable<TEntity> GetAll();

        /// <summary>
        /// Get Entity by id.
        /// </summary>
        /// <param name="id">object id.</param>
        /// <returns>entity.</returns>
        Task<TEntity> GetByIdAsync(object id);

        /// <summary>
        /// Insert.
        /// </summary>
        /// <param name="entity">entity.</param>
        void Insert(TEntity entity);

        /// <summary>
        /// Insert.
        /// </summary>
        /// <param name="entityToUpdate">entity.</param>
        void Update(TEntity entityToUpdate);

        /// <summary>
        /// Save the context.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task SaveAsync();

        /// <summary>
        /// Get total record count.
        /// </summary>
        /// <returns>record count.</returns>
        int GetTotalCount();
    }
}
