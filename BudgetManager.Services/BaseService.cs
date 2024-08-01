using AutoMapper;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Repository;
using BudgetManager.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace BudgetManager.Services
{
    /// <summary>
    /// Base servie model
    /// </summary>
    /// <typeparam name="Model">Data type of model</typeparam>
    /// <typeparam name="Entity">Data type of entity</typeparam>
    /// <typeparam name="IRepo">Type of repository</typeparam>
    public class BaseService<Model, Entity, IRepo> : IBaseService<Model, Entity, IRepo>
        where Model : IDtoModel
        where Entity : class, IDataModel
        where IRepo : IRepository<Entity>
    {
        protected readonly IRepo repository;
        protected readonly IMapper mapper;

        /// <summary>
        /// Create instance of service
        /// </summary>
        /// <param name="repository">Repository for model</param>
        /// <param name="mapper">Auto mapper instance</param>
        public BaseService(IRepo repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        /// <summary>
        /// Method to add database record
        /// </summary>
        /// <param name="model">Entity model</param>
        /// <returns>Id of record in database</returns>
        public virtual int Add(Model model)
        {
            Entity entity = mapper.Map<Entity>(model);
            entity.Id = default;
            repository.Create(entity);
            repository.Save();
            return entity.Id;
        }

        /// <summary>
        /// Method to update database record
        /// </summary>
        /// <param name="model">Model to update</param>
        /// <exception cref="Exception">Model with specific Id not found</exception>
        public virtual void Update(Model model)
        {
            if (!repository.FindByCondition(p => p.Id == model.Id).Any())
                throw new Exception();

            Entity entity = mapper.Map<Entity>(model);
            repository.Update(entity);
            repository.Save();
        }

        /// <summary>
        /// Method to delete database record
        /// </summary>
        /// <param name="model">Model to remove</param>
        public virtual void Delete(int id)
        {
            Entity entity = repository.FindByCondition(a => a.Id == id).Single();
            repository.Delete(entity);
            repository.Save();
        }

        /// <summary>
        /// Method to get entity by id
        /// </summary>
        /// <param name="id">Id of entity</param>
        /// <returns>Model of entity</returns>
        public virtual Model Get(int id)
        {
            Entity entity = repository.FindByCondition(p => p.Id == id).Single();
            return mapper.Map<Model>(entity);
        }

        /// <summary>
        /// Method to get entity aftre applied filter
        /// </summary>
        /// <param name="expression">Filter expression</param>
        /// <returns>Records after filtering</returns>
        public virtual IEnumerable<Model> Get(Expression<Func<Entity, bool>> expression) 
            => repository.FindByCondition(expression).Select(a => mapper.Map<Model>(a));

        /// <summary>
        /// Method to get all records
        /// </summary>
        /// <returns>All entity models</returns>
        public virtual IEnumerable<Model> GetAll() => repository.FindAll().Select(a => mapper.Map<Model>(a));
    }
}
