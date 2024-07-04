using BudgetManager.Data.DataModels;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace BudgetManager.Repository
{
    /// <summary>
    /// Provides a generic repository interface for managing entities of type T.
    /// </summary>
    /// <typeparam name="T">The entity type that must be a class and implement IDataModel.</typeparam>
    public interface IRepository<T> where T : class, IDataModel
    {
        /// <summary>
        /// Retrieves all entities of type T.
        /// </summary>
        /// <returns>An IQueryable collection of entities.</returns>
        IQueryable<T> FindAll();

        /// <summary>
        /// Retrieves entities of type T that satisfy the specified condition.
        /// </summary>
        /// <param name="expression">A lambda expression representing the condition to filter the entities.</param>
        /// <returns>An IQueryable collection of entities that satisfy the condition.</returns>
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression);

        /// <summary>
        /// Adds a new entity of type T to the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        void Create(T entity);

        /// <summary>
        /// Updates an existing entity of type T in the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        void Update(T entity);

        /// <summary>
        /// Deletes an entity of type T from the repository.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        void Delete(T entity);

        /// <summary>
        /// Saves all changes made in the context of the repository.
        /// </summary>
        void Save();

        /// <summary>
        /// Qeury using formatable string
        /// </summary>
        /// <typeparam name="ET">Return model type</typeparam>
        /// <param name="sql">Formatable string with qeury</param>
        /// <returns>Queryable of return models</returns>
        IQueryable<ET> FromSql<ET>(FormattableString sql);

        /// <summary>
        /// Retrieves an entity of type T by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity to retrieve.</param>
        /// <returns>The entity found with the given identifier.</returns>
        T Get(int id);
    }
}
