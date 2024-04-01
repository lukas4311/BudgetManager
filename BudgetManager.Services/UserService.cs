using AutoMapper;
using BudgetManager.Data.DataModels;
using BudgetManager.Domain.DTOs;
using BudgetManager.Repository;
using BudgetManager.Services.Contracts;
using System.Linq;

namespace BudgetManager.Services
{
    public class UserService : IUserService
    {
        private readonly IUserIdentityRepository userIdentityRepository;
        private readonly IUserDataRepository userDataRepository;
        private readonly IMapper mapper;
        private readonly IHashManager hashManager;

        public UserService(IUserIdentityRepository userIdentityRepository, IHashManager hashManager, IUserDataRepository userDataRepository, IMapper mapper)
        {
            this.userIdentityRepository = userIdentityRepository;
            this.hashManager = hashManager;
            this.userDataRepository = userDataRepository;
            this.mapper = mapper;
        }

        public UserIdentification Authenticate(string username, string password) => this.userIdentityRepository.FindByCondition(x => x.Login == username && x.PasswordHash == this.hashManager.HashPasswordToSha512(password)).Select(u => new UserIdentification
        {
            UserId = u.Id,
            UserName = u.Login
        }).SingleOrDefault();

        public int GetUserId(string userLogin) => this.userIdentityRepository.FindByCondition(a => a.Login == userLogin).Single().Id;

        public void CreateUser(UserCreateModel userCreateModel)
        {
            UserData userData = this.mapper.Map<UserData>(userCreateModel);
            UserIdentity userIdentity = this.mapper.Map<UserIdentity>(userCreateModel);
            userIdentity.UserData = userData;
            this.userDataRepository.Create(userData);
            this.userIdentityRepository.Create(userIdentity);
            //this.userIdentityRepository.Save();
        }
    }
}
