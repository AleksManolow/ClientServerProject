using ClientServerProject.Server.Models;
using ClientServerProject.Server.Repositories;

namespace ClientServerProject.Server
{
    public class Program
    {
        static void Main(string[] args)
        {
            string connectionString = @"Server=DESKTOP-AJ5FISA\SQLEXPRESS;Database=ClientServerProject;Integrated Security = True;TrustServerCertificate=True;";

            var userRepository = new UserRepository(connectionString);

            userRepository.InitializeDatabase();

            var httpServer = new HttpServer(userRepository);
            httpServer.Start();

            Console.WriteLine("Server started. Press any key to stop...");
            Console.ReadKey();

            httpServer.Stop();

        }
    }
}
