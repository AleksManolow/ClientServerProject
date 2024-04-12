﻿using ClientServerProject.Server.Models;
using ClientServerProject.Server.Repositories;
using ClientServerProject.Server.Repositories.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

            if (_userRepository.GetById(user.Id) == null)
            {
                response.ContentType = "text/plain";
                response.OutputStream.Write(Encoding.UTF8.GetBytes("User with this id does not exist"), 0, Encoding.UTF8.GetBytes("User with this id does not exist").Length);
                response.Close();
                return;
            }

            try
            {
                _userRepository.UpdateUser(user);

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
            string streamReader = new StreamReader(request.InputStream).ReadToEnd();

            UserIdForm userIdForm = JsonConvert.DeserializeObject<UserIdForm>(streamReader)!;

            if (_userRepository.GetById(userIdForm.Id) == null)
            {
                response.ContentType = "text/plain";
                response.OutputStream.Write(Encoding.UTF8.GetBytes("User with this id does not exist"), 0, Encoding.UTF8.GetBytes("User with this id does not exist").Length);
                response.Close();
                return;
            }

            try
            {
                _userRepository.DeleteUser(userIdForm.Id);

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
            string streamReader = new StreamReader(request.InputStream).ReadToEnd();

            UserIdForm userIdForm = JsonConvert.DeserializeObject<UserIdForm>(streamReader)!;

            if (_userRepository.GetById(userIdForm.Id) == null)
            {
                response.ContentType = "text/plain";
                response.OutputStream.Write(Encoding.UTF8.GetBytes("User with this id does not exist"), 0, Encoding.UTF8.GetBytes("User with this id does not exist").Length);
                response.Close();
                return;
            }

            try
            {
                User user = _userRepository.GetById(userIdForm.Id);

                response.StatusCode = (int)HttpStatusCode.OK;
                response.ContentType = "application/json";
                var userjson = JsonConvert.SerializeObject(user);
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
