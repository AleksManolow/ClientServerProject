using ClientServerProject.Server.Repositories;
using ClientServerProject.Server.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ClientServerProject.Server.Controllers
{
    public class UserController
    {
        private readonly IUserRepository _userRepository;
        private readonly Validation _validation;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _validation = new Validation();
        }
        public async Task RegisterUserAsync(HttpListenerContext context)
        {
            Console.WriteLine("Register");
        }
        public async Task LoginUserAsync(HttpListenerContext context)
        {
            Console.WriteLine("Login");
        }
    }
}
