using ClientServerProject.Server.Controllers;
using ClientServerProject.Server.Repositories;
using System.Net;

namespace ClientServerProject.Server
{
    public class HttpServer
    {
        private readonly HttpListener _listener;
        private readonly UserRepository _userRepository;

        public HttpServer(UserRepository userRepository)
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add("http://localhost:8075/");
            _userRepository = userRepository;
        }

        public void Start()
        {
            _listener.Start();
            Console.WriteLine("HTTP server started.");
            _listener.BeginGetContext(HandleRequest, null);
        }

        public void Stop()
        {
            _listener.Stop();
            _listener.Close();
            Console.WriteLine("HTTP server stopped.");
        }

        private void HandleRequest(IAsyncResult result)
        {
            var context = _listener.EndGetContext(result);

            var authController = new AuthController(_userRepository);
            var userContrller = new UserController(_userRepository);

            switch (context.Request.Url!.AbsolutePath)
            {
                case "/register":
                    authController.Register(context);
                    break;
                case "/login":
                    authController.Login(context);
                    break;
                case "/logout":
                    authController.Logout(context);
                    break;
                case "/update":
                    userContrller.Update(context);
                    break;
                case "/delete":
                    userContrller.Delete(context);
                    break;
                case "/get":
                    userContrller.Get(context);
                    break;
                default:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    context.Response.Close();
                    break;
            }

            _listener.BeginGetContext(HandleRequest, null);
        }
    }
}
