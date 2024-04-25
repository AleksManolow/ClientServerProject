using ClientServerProject.Server.Models;
using ClientServerProject.Server.Services;

namespace ClientServerProject.Server
{
    public class Program
    {
        static void Main(string[] args)
        {
            string connectionString = @"Server=DESKTOP-AJ5FISA\SQLEXPRESS;Database=ClientServerProject;Integrated Security = True;TrustServerCertificate=True;";

            var userService = new UserService(connectionString);

            userService.InitializeDatabase();

            var httpServer = new HttpServer(userService);
            httpServer.Start();

            Console.WriteLine("Server started. Press any key to stop!");
            Console.ReadKey();

            httpServer.Stop();
        }
    }
}
