using BudgetManager.Repository;
using BudgetManager.Services.Contracts;
using System.Collections.Generic;

namespace BudgetManager.Services
{
    internal abstract class BaseService<Model, Entity, IRepo> : IBaseService<Model> where IRepo : IRepository<Entity>
    {
        private readonly IRepo repository;

        public BaseService(IRepo repository)
        {
            this.repository = repository;
        }

        public int Add(Model model)
        {
            //map model to entity
            // add entity to repo
            // save context

            throw new System.NotImplementedException();
        }

        public void Update(Model model)
        {
            throw new System.NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new System.NotImplementedException();
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
