using NUnit.Framework;
using RestSharp;
using System.Text.Json;
using System.Net;

namespace RevueCrafters.Tests
{
    public class BaseTest
    {
        protected static RestClient? client;
        protected static string baseUrl = "https://d2925tksfvgq8c.cloudfront.net/api";
        protected static string? token;
        protected static string? lastRevueId;

        [OneTimeSetUp]
        public void Setup()
        {
            client = new RestClient(baseUrl);

            // Login
            var request = new RestRequest("/User/Authentication", Method.Post);
            request.AddJsonBody(new
            {
                email = "htsurev@gmail.com",
                password = "asdasd"
            });

            var response = client.Execute(request);

            if (response.StatusCode != HttpStatusCode.OK || string.IsNullOrEmpty(response.Content))
            {
                throw new Exception($"Login failed: {response.StatusCode} - {response.Content}");
            }

            var json = JsonDocument.Parse(response.Content);
            token = json.RootElement.GetProperty("accessToken").GetString();
        }

        // Метод за добавяне на Authorization header
        protected void AddAuthHeader(RestRequest request)
        {
            if (!string.IsNullOrEmpty(token))
            {
                request.AddHeader("Authorization", $"Bearer {token}");
            }
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            client?.Dispose();
        }
    }
}