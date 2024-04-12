using ClientServerProject.Server;
using ClientServerProject.Server.Controllers;
using ClientServerProject.Server.Repositories;
using ClientServerProject.Server.Repositories.Interfaces;
using Moq;
using System.Net;
using System.Net.Http.Json;
using System.Text;

namespace ClientServerProject.Tests
{
    public class UserControllerTests
    {
        private const string BaseUrl = "http://localhost:8075/";
        private HttpClient _httpClient;
        private HttpServer _httpServer;

        [SetUp]
        public void Setup()
        {
            _httpServer = new HttpServer(new UserRepository(@"Server=DESKTOP-AJ5FISA\SQLEXPRESS;Database=ClientServerProject;Integrated Security = True;TrustServerCertificate=True;"));
            _httpServer.Start();
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        }

        [TearDown]
        public void TearDown()
        {
            _httpServer.Stop();
        }

        [Test]
        public async Task UpdateUserSuccess()
        {

            var userJson = "{\"Id\": \"6\", \"Email\": \"test@test.com\", \"FirstName\": \"Kalin\", \"LastName\": \"Ivanov\", \"Password\": \"Test1234\"}";
            var content = new StringContent(userJson, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync("/update", content);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            Assert.AreEqual("User updated successfully", response.Content.ReadAsStringAsync().Result.ToString());
        }
        [Test]
        public async Task UpdateUserInvalidDate()
        {

            var userJson = "{\"Id\": \"6\", \"Email\": \"test@te213st.cad12som\", \"FirstName\": \"Kalin\", \"LastName\": \"Ivanov\", \"Password\": \"Test\"}";
            var content = new StringContent(userJson, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync("/update", content);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

            Assert.AreEqual("Invalid user data", response.Content.ReadAsStringAsync().Result.ToString());
        }
        [Test]
        public async Task UpdateUserNotExist()
        {
            var userJson = "{\"Id\": \"1\", \"Email\": \"test@test.com\", \"FirstName\": \"Kalin\", \"LastName\": \"Ivanov\", \"Password\": \"Test1234\"}";
            var content = new StringContent(userJson, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync("/update", content);

            Assert.AreEqual("User with this id does not exist", response.Content.ReadAsStringAsync().Result.ToString());
        }
        [Test]
        public async Task DeleteUserSuccess()
        {
            var response = await _httpClient.DeleteAsync("/delete/id=5");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            Assert.AreEqual("User delete successfully", response.Content.ReadAsStringAsync().Result.ToString());
        }
        [Test]
        public async Task DeleteUserNotExist()
        {
            var response = await _httpClient.DeleteAsync("/delete/id=5");

            Assert.AreEqual("User with this id does not exist", response.Content.ReadAsStringAsync().Result.ToString());
        }
        [Test]
        public async Task GetUserSuccess()
        {
            var response = await _httpClient.GetAsync("/get/id=6");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }
        [Test]
        public async Task GetUserNotExist()
        {
            var response = await _httpClient.GetAsync("/get/id=6");

            Assert.AreEqual("User with this id does not exist", response.Content.ReadAsStringAsync().Result.ToString());
        }

    }
}