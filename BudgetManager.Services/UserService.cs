using BudgetManager.Domain.DTOs;
using BudgetManager.Repository;
using BudgetManager.Services.Contracts;
using System.Linq;

namespace BudgetManager.Services
{
    public class UserService : IUserService
    {
        private readonly IUserIdentityRepository userIdentityRepository;
        private readonly IHashManager hashManager;

        public UserService(IUserIdentityRepository userIdentityRepository, IHashManager hashManager)
        {
            this.userIdentityRepository = userIdentityRepository;
            this.hashManager = hashManager;
        }

        public UserIdentification Authenticate(string username, string password) => this.userIdentityRepository.FindByCondition(x => x.Login == username && x.PasswordHash == this.hashManager.HashPasswordToSha512(password)).Select(u => new UserIdentification
        {
            UserId = u.Id,
            UserName = u.Login
        }).SingleOrDefault();

        public int GetUserId(string userLogin) => this.userIdentityRepository.FindByCondition(a => a.Login == userLogin).Single().Id;
    }
}
