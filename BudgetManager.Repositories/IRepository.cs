using BudgetManager.Data.DataModels;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace BudgetManager.Repository
{
    public interface IRepository<T> where T : class, IDataModel
    {
        IQueryable<T> FindAll();

        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression);

        void Create(T entity);

        void Update(T entity);

        void Delete(T entity);

        void Save();
        T Get(int id);
    }
}
