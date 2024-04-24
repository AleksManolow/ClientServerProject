using ClientServerProject.Server;
using ClientServerProject.Server.Controllers;
using ClientServerProject.Server.Models;
using ClientServerProject.Server.Repositories;
using Moq;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace ClientServerProject.Tests
{
    public class AuthControllerTests
    {

        private const string BaseUrl = "http://localhost:8075/";
        private HttpClient _httpClient;
        private HttpServer _httpServer;

        [SetUp]
        public void Setup()
        {
            _httpServer = new HttpServer(new UserService(@"Server=DESKTOP-AJ5FISA\SQLEXPRESS;Database=ClientServerProject;Integrated Security = True;TrustServerCertificate=True;"));
            _httpServer.Start();
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        }

        [TearDown]
        public void TearDown()
        {
            _httpServer.Stop();
        }

        [Test]
        public async Task RegisterUserSuccess()
        {

            var userJson = "{\"Email\": \"test2156@abv.bg\", \"FirstName\": \"Gosho\", \"LastName\": \"Petrov\", \"Password\": \"Goshoo1234\"}";
            var content = new StringContent(userJson, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/register", content);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            
            Assert.AreEqual("User registered successfully", response.Content.ReadAsStringAsync().Result.ToString());
        }
        [Test]
        public async Task RegisterUserInvalidaDate()
        {
            var userJson = "{\"Email\": \"test@test213.com12!\", \"FirstName\": \"Test\", \"LastName\": \"User\", \"Password\": \"Test1234\"}";
            var content = new StringContent(userJson, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/register", content);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

            Assert.AreEqual("Invalid user data", response.Content.ReadAsStringAsync().Result.ToString());
        }
        [Test]
        public async Task RegisterUserWithEmailAlreadyRegistered()
        {
            var userJson = "{\"Email\": \"test213456@abv.bg\", \"FirstName\": \"Test\", \"LastName\": \"User\", \"Password\": \"Test1234\"}";
            var content = new StringContent(userJson, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/register", content);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

            Assert.AreEqual("User with this email is already registered", response.Content.ReadAsStringAsync().Result.ToString());
        }
        [Test]
        public async Task LoginUserSuccess()
        {
            var userJson = "{\"Email\": \"test213456@abv.bg\", \"Password\": \"Goshoo1234\"}";
            var content = new StringContent(userJson, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/login", content);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
        [Test]
        public async Task LoginUserInvalidaDate()
        {
            var userJson = "{\"Email\": \"test@test213.com12!\", \"Password\": \"Goshoo1234\"}";
            var content = new StringContent(userJson, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/login", content);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            Assert.AreEqual("Invalid user data", response.Content.ReadAsStringAsync().Result.ToString());
        }
        [Test]
        public async Task LoginUserUnauthorized()
        {
            var userJson = "{\"Email\": \"test123123213456@abv.bg\", \"Password\": \"Goshoo1234\"}";
            var content = new StringContent(userJson, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/login", content);

            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        [Test]
        public async Task LogoutUserSuccess()
        {
            var userJson = "{\"Email\": \"test213456@abv.bg\", \"Password\": \"Goshoo1234\"}";
            var content = new StringContent(userJson, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("/login", content);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", response.Content.ReadAsStringAsync().Result.ToString());

            var response21 = await _httpClient.PostAsync("/logout", null);

            Assert.AreEqual(HttpStatusCode.OK, response21.StatusCode);
        }
    }
}
