using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace BudgetManager.Services.Contracts
{
    public interface IBaseService<Model, Entity, IRepo>
    {
        int Add(Model model);

        void Update(Model model);

        void Delete(int id);

        IEnumerable<Model> GetAll();

        Model Get(int id);

        IEnumerable<Model> Get(Expression<Func<Entity, bool>> expression);
    }
}
