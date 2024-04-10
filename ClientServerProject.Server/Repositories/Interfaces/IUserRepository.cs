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
        void AddUser(User user);
    }
}
