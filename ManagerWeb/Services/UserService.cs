using ManagerWeb.Models;
using Repository;
using System.Linq;
using System.Threading.Tasks;

namespace ManagerWeb.Services
{
    public class UserService : IUserService
    {
        private readonly IUserIdentityRepository userIdentityRepository;

        public UserService(IUserIdentityRepository userIdentityRepository)
        {
            this.userIdentityRepository = userIdentityRepository;
        }

        public async Task<UserModel> Authenticate(string username, string passwordHash)
        {
            return await Task.Run(() => this.userIdentityRepository.FindByCondition(x => x.Login == username && x.PasswordHash == passwordHash).Select(a => new UserModel { Login = a.Login }).SingleOrDefault()).ConfigureAwait(false);
        }
    }
}
