using ClientServerProject.Server.Controllers;
using ClientServerProject.Server.Services;
using System.Net;

namespace ClientServerProject.Server
{
    public class HttpServer
    {
        private readonly HttpListener _listener;
        private readonly UserService _userService;

        public HttpServer(UserService userService)
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add("http://localhost:8075/");
            _userService = userService;
        }

        public void Start()
        {
            _listener.Start();
            _listener.BeginGetContext(HandleRequest, null);
        }

        public void Stop()
        {
            _listener.Stop();
            _listener.Close();
        }

        private void HandleRequest(IAsyncResult result)
        {
            var context = _listener.EndGetContext(result);

            var authController = new AuthController(_userService);
            var userContrller = new UserController(_userService);

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
