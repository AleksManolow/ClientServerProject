﻿using ClientServerProject.Server.Models;
using ClientServerProject.Server.Services;
using ClientServerProject.Server.Services.Interfaces;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;

namespace ClientServerProject.Server.Controllers
{
    public class UserController
    {
        private readonly IUserService _userService;
        private readonly Validation _validation;

        public UserController(IUserService userService)
        {
            _userService = userService;
            _validation = new Validation();
        }

        public async Task Update(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;

            if (request.HttpMethod != "PUT")
            {
                response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                response.Close();
                return;
            }

            var tokenString = request.Headers["Authorization"]?.Replace("Bearer ", "");

            if (string.IsNullOrEmpty(tokenString))
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.ContentType = "text/plain";
                response.OutputStream.Write(Encoding.UTF8.GetBytes("Invalid JWT token."), 0, Encoding.UTF8.GetBytes("Invalid JWT token.").Length);
                response.Close();
                return;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(tokenString);

            var user = _userService.GetById(int.Parse(token.Claims.FirstOrDefault(t => t.Type == "nameid").Value));
            if (user == null)
            {
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response.ContentType = "text/plain";
                response.OutputStream.Write(Encoding.UTF8.GetBytes("Unauthorized user."), 0, Encoding.UTF8.GetBytes("Unauthorized user.").Length);
                response.Close();
                return;
            }

            string streamReader = new StreamReader(request.InputStream).ReadToEnd();

            User userInfo = JsonConvert.DeserializeObject<User>(streamReader)!;

            if (!_validation.ValidateEmail(userInfo.Email) || !_validation.ValidateName(userInfo.FirstName) || !_validation.ValidateName(userInfo.LastName) || !_validation.ValidatePassword(userInfo.Password))
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.ContentType = "text/plain";
                response.OutputStream.Write(Encoding.UTF8.GetBytes("Invalid user data"), 0, Encoding.UTF8.GetBytes("Invalid user data").Length);
                response.Close();
                return;
            }

            if (_userService.GetById(userInfo.Id) == null)
            {
                response.ContentType = "text/plain";
                response.OutputStream.Write(Encoding.UTF8.GetBytes("User with this id does not exist"), 0, Encoding.UTF8.GetBytes("User with this id does not exist").Length);
                response.Close();
                return;
            }

            try
            {
                _userService.UpdateUser(userInfo);

                response.StatusCode = (int)HttpStatusCode.OK;
                response.ContentType = "text/plain";
                response.OutputStream.Write(Encoding.UTF8.GetBytes("User updated successfully"), 0, Encoding.UTF8.GetBytes("User updated successfully").Length);
                response.Close();
            }
            catch (Exception)
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.ContentType = "text/plain";
                response.OutputStream.Write(Encoding.UTF8.GetBytes("Unexpected error occurred while trying to update user! Please try again later!"), 0, Encoding.UTF8.GetBytes("Unexpected error occurred while trying to update user! Please try again later!").Length);
                response.Close();
            }
        }
        public async Task Delete(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;

            if (request.HttpMethod != "DELETE")
            {
                response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                response.Close();
                return;
            }

            var tokenString = request.Headers["Authorization"]?.Replace("Bearer ", "");

            if (string.IsNullOrEmpty(tokenString))
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.ContentType = "text/plain";
                response.OutputStream.Write(Encoding.UTF8.GetBytes("Invalid JWT token."), 0, Encoding.UTF8.GetBytes("Invalid JWT token.").Length);
                response.Close();
                return;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(tokenString);

            var user = _userService.GetById(int.Parse(token.Claims.FirstOrDefault(t => t.Type == "nameid").Value));
            if (user == null)
            {
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response.ContentType = "text/plain";
                response.OutputStream.Write(Encoding.UTF8.GetBytes("Unauthorized user."), 0, Encoding.UTF8.GetBytes("Unauthorized user.").Length);
                response.Close();
                return;
            }

            string streamReader = new StreamReader(request.InputStream).ReadToEnd();

            UserIdForm userIdForm = JsonConvert.DeserializeObject<UserIdForm>(streamReader)!;

            if (_userService.GetById(userIdForm.Id) == null)
            {
                response.ContentType = "text/plain";
                response.OutputStream.Write(Encoding.UTF8.GetBytes("User with this id does not exist"), 0, Encoding.UTF8.GetBytes("User with this id does not exist").Length);
                response.Close();
                return;
            }

            try
            {
                _userService.DeleteUser(userIdForm.Id);

                response.StatusCode = (int)HttpStatusCode.OK;
                response.ContentType = "text/plain";
                response.OutputStream.Write(Encoding.UTF8.GetBytes("User delete successfully"), 0, Encoding.UTF8.GetBytes("User delete successfully").Length);
                response.Close();
            }
            catch (Exception)
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.ContentType = "text/plain";
                response.OutputStream.Write(Encoding.UTF8.GetBytes("Unexpected error occurred while trying to delete user! Please try again later!"), 0, Encoding.UTF8.GetBytes("Unexpected error occurred while trying to delete user! Please try again later!").Length);
                response.Close();
            }
        }
        public async Task Get(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;

            if (request.HttpMethod != "GET")
            {
                response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                response.Close();
                return;
            }

            var tokenString = request.Headers["Authorization"]?.Replace("Bearer ", "");

            if (string.IsNullOrEmpty(tokenString))
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.ContentType = "text/plain";
                response.OutputStream.Write(Encoding.UTF8.GetBytes("Invalid JWT token."), 0, Encoding.UTF8.GetBytes("Invalid JWT token.").Length);
                response.Close();
                return;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(tokenString);

            var user = _userService.GetById(int.Parse(token.Claims.FirstOrDefault(t => t.Type == "nameid").Value));
            if (user == null)
            {
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response.ContentType = "text/plain";
                response.OutputStream.Write(Encoding.UTF8.GetBytes("Unauthorized user."), 0, Encoding.UTF8.GetBytes("Unauthorized user.").Length);
                response.Close();
                return;
            }

            string streamReader = new StreamReader(request.InputStream).ReadToEnd();

            UserIdForm userIdForm = JsonConvert.DeserializeObject<UserIdForm>(streamReader)!;

            if (_userService.GetById(userIdForm.Id) == null)
            {
                response.ContentType = "text/plain";
                response.OutputStream.Write(Encoding.UTF8.GetBytes("User with this id does not exist"), 0, Encoding.UTF8.GetBytes("User with this id does not exist").Length);
                response.Close();
                return;
            }

            try
            {
                User userInfo = _userService.GetById(userIdForm.Id);

                response.StatusCode = (int)HttpStatusCode.OK;
                response.ContentType = "application/json";
                var userjson = JsonConvert.SerializeObject(userInfo);
                response.OutputStream.Write(Encoding.UTF8.GetBytes(userjson), 0, Encoding.UTF8.GetBytes(userjson).Length);
                response.Close();
            }
            catch (Exception)
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.ContentType = "text/plain";
                response.OutputStream.Write(Encoding.UTF8.GetBytes("Unexpected error occurred while trying to get user! Please try again later!"), 0, Encoding.UTF8.GetBytes("Unexpected error occurred while trying to get user! Please try again later!").Length);
                response.Close();
            }
        }
    }
}
