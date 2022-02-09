using System.Collections.Generic;

namespace BudgetManager.Services.Contracts
{
    public interface IBaseService<Model>
    {
        int Add(Model model);

        void Update(Model model);

        void Delete(int id);

        IEnumerable<Model> GetAll();

        Model Get(int id);
    }
}
