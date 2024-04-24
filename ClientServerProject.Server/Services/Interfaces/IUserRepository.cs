using ClientServerProject.Server.Models;

namespace ClientServerProject.Server.Services.Interfaces
{
    public interface IUserService
    {
        public void InitializeDatabase();
        User GetById(int id);
        User GetByEmail(string email);
        void CreateUser(User user);
        void UpdateUser(User user);
        void DeleteUser(int userId);
        public string GenerateJwt(User user);
        public void SendVerificationEmail(string recipientEmail);
    }
}
