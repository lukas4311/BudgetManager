﻿using BudgetManager.Data;
using BudgetManager.Data.DataModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace BudgetManager.Repository
{
    public abstract class Repository<T> : IRepository<T> where T : class, IDataModel
    {
        protected DataContext RepositoryContext { get; set; }

        public Repository(DataContext repositoryContext) => this.RepositoryContext = repositoryContext;

        public IQueryable<T> FindAll() => this.RepositoryContext.Set<T>().AsNoTracking();

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression) => this.RepositoryContext.Set<T>().Where(expression).AsNoTracking();

        public void Create(T entity) => this.RepositoryContext.Set<T>().Add(entity);

        public void Update(T entity) => this.RepositoryContext.Set<T>().Update(entity);

        public void Delete(T entity) => this.RepositoryContext.Set<T>().Remove(entity);

        public void Save() => this.RepositoryContext.SaveChanges();
    }
}
