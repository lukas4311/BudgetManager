using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Repository;
using BudgetManager.Services.Contracts;
using System.Collections.Generic;
using System.Linq;

namespace BudgetManager.Services
{
    internal abstract class BaseService<Model, Entity, IRepo> : IBaseService<Model>
        where Model : IDtoModel<Entity>
        where Entity : class, IDataModel
        where IRepo : IRepository<Entity>
    {
        private readonly IRepo repository;

        public BaseService(IRepo repository)
        {
            this.repository = repository;
        }

        public int Add(Model model)
        {
            var entity = model.ToEntity();
            this.repository.Create(entity);
            this.repository.Save();
            return entity.Id;
        }

        public void Update(Model model)
        {
            Entity bankAccount = this.repository.FindByCondition(p => p.Id == model.Id).Single();
            //Entity model = model.ToEntity();

            this.repository.Update(bankAccount);
            this.repository.Save();
        }

        public void Delete(int id)
        {
            Entity entity = this.repository.FindByCondition(a => a.Id == id).Single();
            this.repository.Delete(entity);
            this.repository.Save();
        }

        public Model Get(int id)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Model> GetAll()
        {
            throw new System.NotImplementedException();
        }
    }
}
