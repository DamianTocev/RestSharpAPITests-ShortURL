using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using RestSharp;
using System;
using System.Net;
using System.Text.Json;

namespace RestSharpAPITests
{
    public class RestSharpAPI_Tests
    {
        private RestClient client;
        private const string baseUrl = "https://shorturl.damiant4.repl.co/api";

        [SetUp]
        public void Setup()
        {
            this.client = new RestClient(baseUrl);
        }


        [Test]
        public void Test_Get_List_all_urls()
        {
            // Arrange
            var request = new RestRequest("urls", Method.Get);

            // Act
            var response = this.client.Execute(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var tasks = JsonSerializer.Deserialize<List<urls>>(response.Content);
            Assert.That(response.Content, Is.Not.Empty);
            //Assert.That(tasks[0].url, Is.EqualTo("https://nakov.com"));
            //Assert.That(tasks[1].url, Is.EqualTo("https://selenium.dev"));

        }

        [Test]
        public void Test_Get_nak_url()
        {
            // Arrange
            var request = new RestRequest("urls/nak", Method.Get);

            // Act
            var response = this.client.Execute(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            var tasks = JsonSerializer.Deserialize<urls>(response.Content);
            Assert.That(tasks.url, Is.EqualTo("https://nakov.com"));
            Assert.That(tasks.shortCode, Is.EqualTo("nak"));

        }

        [Test]
        public void Test_GET_BY_Invalid_URL()
        {
            // Arrange
            var request = new RestRequest("urls/notfound", Method.Get);

            // Act
            var response = this.client.Execute(request);

            // Assert
            //Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(response.Content, Is.EqualTo("{\"errMsg\":\"Short code not found: notfound\"}"));
        }

        [Test]
        public void Test_Create_URL()
        {
            // Arrange
            var request = new RestRequest("urls", Method.Post);
            var reqBody = new
            {
                url = "https://cnn.com" + DateTime.Now.Ticks,
                shortCode = "cnn" + DateTime.Now.Ticks
            };
            request.AddBody(reqBody);

            // Act
            var response = this.client.Execute(request);
            
            var taskObject1 = JsonSerializer.Deserialize<taskObject>(response.Content);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            Assert.That(taskObject1.msg, Is.EqualTo("Short code added."));

            Assert.That(taskObject1.url.url, Is.EqualTo(reqBody.url));

            Assert.That(taskObject1.url.shortCode, Is.EqualTo(reqBody.shortCode));
        }

        [Test]
        public void Test_Create_URL_Duplicate()
        {
            // Arrange
            var request = new RestRequest("urls", Method.Post);
            var reqBody = new
            {
                url = "https://nakov.com",
                shortCode = "nak"
            };
            request.AddBody(reqBody);

            // Act
            var response = this.client.Execute(request);

            var taskObject1 = JsonSerializer.Deserialize<taskObject>(response.Content);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }
    }
}