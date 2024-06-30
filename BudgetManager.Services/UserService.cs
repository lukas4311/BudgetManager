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
        private readonly IRepository<UserIdentity> userIdentityRepository;
        private readonly IRepository<UserData> userDataRepository;
        private readonly IMapper mapper;
        private readonly IHashManager hashManager;

        public UserService(IRepository<UserIdentity> userIdentityRepository, IHashManager hashManager, IRepository<UserData> userDataRepository, IMapper mapper)
        {
            this.userIdentityRepository = userIdentityRepository;
            this.hashManager = hashManager;
            this.userDataRepository = userDataRepository;
            this.mapper = mapper;
        }

        /// <inheritdoc/>
        public UserIdentification Authenticate(string username, string password) => userIdentityRepository.FindByCondition(x => x.Login == username && x.PasswordHash == hashManager.HashPasswordToSha512(password)).Select(u => new UserIdentification
        {
            UserId = u.Id,
            UserName = u.Login
        }).SingleOrDefault();

        /// <inheritdoc/>
        public int GetUserId(string userLogin) => userIdentityRepository.FindByCondition(a => a.Login == userLogin).Single().Id;

        /// <inheritdoc/>
        public void CreateUser(UserCreateModel userCreateModel)
        {
            UserData userData = mapper.Map<UserData>(userCreateModel);
            UserIdentity userIdentity = mapper.Map<UserIdentity>(userCreateModel);
            userIdentity.UserData = userData;
            userIdentity.PasswordHash = hashManager.HashPasswordToSha512(userCreateModel.Password);
            userData.IsLocked = false;
            userDataRepository.Create(userData);
            userIdentityRepository.Create(userIdentity);
            userIdentityRepository.Save();
        }
    }
}
