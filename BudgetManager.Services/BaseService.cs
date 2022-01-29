using AutoMapper;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Repository;
using BudgetManager.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BudgetManager.Services
{
    public abstract class BaseService<Model, Entity, IRepo> : IBaseService<Model>
        where Model : IDtoModel
        where Entity : class, IDataModel
        where IRepo : IRepository<Entity>
    {
        private readonly IRepo repository;
        private readonly IMapper mapper;

        public BaseService(IRepo repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public virtual int Add(Model model)
        {
            Entity entity = this.mapper.Map<Entity>(model);
            entity.Id = default;
            this.repository.Create(entity);
            this.repository.Save();
            return entity.Id;
        }

        public virtual void Update(Model model)
        {
            if (!this.repository.FindByCondition(p => p.Id == model.Id).Any())
                throw new Exception();

            Entity entity = this.mapper.Map<Entity>(model);
            this.repository.Update(entity);
            this.repository.Save();
        }

        public virtual void Delete(int id)
        {
            Entity entity = this.repository.FindByCondition(a => a.Id == id).Single();
            this.repository.Delete(entity);
            this.repository.Save();
        }

        public Model Get(int id)
        {
            Entity entity = this.repository.FindByCondition(p => p.Id == id).Single();
            return this.mapper.Map<Model>(entity);
        }

        public virtual IEnumerable<Model> GetAll() => this.repository.FindAll().Select(a => this.mapper.Map<Model>(a));
    }
}
