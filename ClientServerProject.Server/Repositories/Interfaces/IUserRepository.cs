using ClientServerProject.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientServerProject.Server.Repositories.Interfaces
{
    public interface IUserRepository
    {
        public void InitializeDatabase();
        User GetById(int id);
        User GetByEmail(string email);
        void CreateUser(User user);
        void UpdateUser(User user);
        void DeleteUser(User user);
        public string GenerateJwt(User user);
    }
}
