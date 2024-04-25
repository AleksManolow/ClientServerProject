using ClientServerProject.Server.Models;
using ClientServerProject.Server.Services;
using ClientServerProject.Server.Services.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;

namespace ClientServerProject.Server.Controllers
{
    public class AuthController
    {
        private readonly IUserService _userService;
        private readonly Validation _validation;

        public AuthController(IUserService userService)
        {
            _userService = userService;
            _validation = new Validation();
        }
        public async Task Register(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;

            if (request.HttpMethod != "POST")
            {
                response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                response.Close();
                return;
            }
            string streamReader = new StreamReader(request.InputStream).ReadToEnd();

            User user = JsonConvert.DeserializeObject<User>(streamReader)!;

            if (!_validation.ValidateEmail(user.Email) || !_validation.ValidateName(user.FirstName) || !_validation.ValidateName(user.LastName) || !_validation.ValidatePassword(user.Password))
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.ContentType = "text/plain";
                response.OutputStream.Write(Encoding.UTF8.GetBytes("Invalid user data"), 0, Encoding.UTF8.GetBytes("Invalid user data").Length);
                response.Close();
                return;
            }

            if (_userService.GetByEmail(user.Email) != null)
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.ContentType = "text/plain";
                response.OutputStream.Write(Encoding.UTF8.GetBytes("User with this email is already registered"), 0, Encoding.UTF8.GetBytes("User with this email is already registered").Length);
                response.Close();
                return;
            }

            try
            {
                _userService.CreateUser(user);

                //Logic to send confirmation email here to create
                //_userService.SendVerificationEmail(user.Email);

                response.StatusCode = (int)HttpStatusCode.OK;
                response.ContentType = "text/plain";
                var responseBytes = Encoding.UTF8.GetBytes("User registered successfully");
                response.OutputStream.Write(responseBytes, 0, responseBytes.Length);
                response.Close();
            }
            catch (Exception)
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.ContentType = "text/plain";
                response.OutputStream.Write(Encoding.UTF8.GetBytes("Unexpected error occurred while trying to register! Please try again later!"), 0, Encoding.UTF8.GetBytes("Unexpected error occurred while trying to register! Please try again later!").Length);
                response.Close();
            }
        }
        public async Task Login(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;

            if (request.HttpMethod != "POST")
            {
                response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                response.Close();
                return;
            }

            string streamReader = new StreamReader(request.InputStream).ReadToEnd();
            UserLoginForm userLoginForm = JsonConvert.DeserializeObject<UserLoginForm>(streamReader)!;

            if (!_validation.ValidateEmail(userLoginForm.Email) || !_validation.ValidatePassword(userLoginForm.Password))
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.ContentType = "text/plain";
                response.OutputStream.Write(Encoding.UTF8.GetBytes("Invalid user data"), 0, Encoding.UTF8.GetBytes("Invalid user data").Length);
                response.Close();
                return;
            }

            var user = _userService.GetByEmail(userLoginForm.Email);
            if (user == null || user.Password != userLoginForm.Password)
            {
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response.Close();
                return;
            }
            
            try
            {
                var token = _userService.GenerateJwt(user);

                response.StatusCode = (int)HttpStatusCode.OK;
                response.ContentType = "application/json";
                var tokenJson = JsonConvert.SerializeObject(new { token });
                response.OutputStream.Write(Encoding.UTF8.GetBytes(tokenJson), 0, Encoding.UTF8.GetBytes(tokenJson).Length);
                response.Close();

            }
            catch (Exception)
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.ContentType = "text/plain";
                response.OutputStream.Write(Encoding.UTF8.GetBytes("Unexpected error occurred while trying to login! Please try again later!"), 0, Encoding.UTF8.GetBytes("Unexpected error occurred while trying to login! Please try again later!").Length);
                response.Close();
            }
        }
        public async Task Logout(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;

            if (request.HttpMethod != "POST")
            {
                response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                response.Close();
                return;
            }

            var tokenString = request.Headers["Authorization"]?.Replace("Bearer ", "");

            if (string.IsNullOrEmpty(tokenString))
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Close();
                return;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(tokenString);

            var user = _userService.GetById(int.Parse(token.Claims.FirstOrDefault(t => t.Type == "nameid").Value));
            if (user == null)
            {
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response.Close();
                return;
            }

            try
            {
                response.StatusCode = (int)HttpStatusCode.OK;
                response.ContentType = "text/plain";
                response.OutputStream.Write(Encoding.UTF8.GetBytes("Logged out successfully"), 0, Encoding.UTF8.GetBytes("Logged out successfully").Length);
                response.Close();
            }
            catch (Exception)
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.ContentType = "text/plain";
                response.OutputStream.Write(Encoding.UTF8.GetBytes("Unexpected error occurred while trying to logout! Please try again later!"), 0, Encoding.UTF8.GetBytes("Unexpected error occurred while trying to logout! Please try again later!").Length);
                response.Close();
            }
        }
    }
}
