using BudgetManager.Repository;
using System.Linq;

namespace BudgetManager.Services
{
    public class UserService
    {
        private readonly IUserIdentityRepository userIdentityRepository;

        public UserService(IUserIdentityRepository userIdentityRepository)
        {
            this.userIdentityRepository = userIdentityRepository;
        }

        public bool Authenticate(string username, string passwordHash) => this.userIdentityRepository.FindByCondition(x => x.Login == username && x.PasswordHash == passwordHash).Count() == 1;

        public int GetUserId(string userLogin) => this.userIdentityRepository.FindByCondition(a => a.Login == userLogin).Single().Id;
    }
}
