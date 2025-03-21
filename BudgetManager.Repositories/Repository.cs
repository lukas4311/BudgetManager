﻿using BudgetManager.Data;
using BudgetManager.Data.DataModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace BudgetManager.Repository
{
    /// <inheritdoc cref="IRepository{T}"/>>
    public class Repository<T> : IRepository<T> where T : class, IDataModel
    {
        /// <summary>
        /// EF context
        /// </summary>
        protected DataContext RepositoryContext { get; set; }

        public Repository(DataContext repositoryContext) => RepositoryContext = repositoryContext;

        /// <inhertidoc/>
        public T Get(int id) => RepositoryContext.Set<T>().SingleOrDefault(e => e.Id == id);

        /// <inhertidoc/>
        public IQueryable<T> FindAll() => RepositoryContext.Set<T>().AsNoTracking();

        /// <inhertidoc/>
        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression) => RepositoryContext.Set<T>().Where(expression).AsNoTracking();

        /// <inhertidoc/>
        public void Create(T entity) => RepositoryContext.Set<T>().Add(entity);

        /// <inhertidoc/>
        public void Update(T entity) => RepositoryContext.Set<T>().Update(entity);

        /// <inhertidoc/>
        public void Delete(T entity) => RepositoryContext.Set<T>().Remove(entity);

        /// <inhertidoc/>
        public IQueryable<ET> FromSql<ET>(FormattableString sql) => RepositoryContext.Database.SqlQuery<ET>(sql);

        /// <inheritdoc />
        public IQueryable<ET> FromSqlRaw<ET>(string sql, params object[] parameters) where ET : class => RepositoryContext.Database.SqlQueryRaw<ET>(sql, parameters);

        /// <inhertidoc/>
        public void Save() => RepositoryContext.SaveChanges();
    }
}
