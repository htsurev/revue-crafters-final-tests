using NUnit.Framework;
using RestSharp;
using RevueCrafters.Tests.DTOs;
using System.Text.Json;

namespace RevueCrafters.Tests
{
    public class RevueTests : BaseTest
    {
        [Test, Order(1)]
        public void CreateRevue_ShouldReturnSuccess()
        {
            var request = new RestRequest("/Revue/Create", Method.Post);
            AddAuthHeader(request);

            var body = new RevueDTO
            {
                Title = "Test Revue",
                Description = "This is a test revue",
                Url = ""
            };
            request.AddJsonBody(body);

            var response = client?.Execute<ApiResponseDTO>(request);

            Assert.That(response?.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
            Assert.That(response?.Data?.Msg, Is.EqualTo("Successfully created!"));
        }

        [Test, Order(2)]
        public void GetAllRevues_ShouldReturnNonEmptyList()
        {
            var request = new RestRequest("/Revue/All", Method.Get);
            AddAuthHeader(request);

            var response = client?.Execute(request);

            Assert.That(response?.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
            Assert.That(response?.Content, Does.Contain("title"));

            if (!string.IsNullOrEmpty(response?.Content))
            {
                var json = JsonDocument.Parse(response.Content);
                lastRevueId = json.RootElement.EnumerateArray().Last().GetProperty("id").GetString();
            }
        }

        [Test, Order(3)]
        public void EditLastRevue_ShouldReturnSuccess()
        {
            var request = new RestRequest($"/Revue/Edit?revueId={lastRevueId}", Method.Put);
            AddAuthHeader(request);

            request.AddJsonBody(new RevueDTO
            {
                Title = "Updated Title",
                Description = "Updated description",
                Url = ""
            });

            var response = client?.Execute<ApiResponseDTO>(request);

            Assert.That(response?.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
            Assert.That(response?.Data?.Msg, Is.EqualTo("Edited successfully"));
        }

        [Test, Order(4)]
        public void DeleteRevue_ShouldReturnSuccess()
        {
            var request = new RestRequest($"/Revue/Delete?revueId={lastRevueId}", Method.Delete);
            AddAuthHeader(request);

            var response = client?.Execute<ApiResponseDTO>(request);

            Assert.That(response?.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.OK));
            Assert.That(response?.Data?.Msg, Is.EqualTo("The revue is deleted!"));
        }

        [Test, Order(5)]
        public void CreateRevue_WithoutRequiredFields_ShouldReturnBadRequest()
        {
            var request = new RestRequest("/Revue/Create", Method.Post);
            AddAuthHeader(request);

            request.AddJsonBody(new { });

            var response = client?.Execute(request);

            Assert.That(response?.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.BadRequest));
        }

        [Test, Order(6)]
        public void EditNonExistingRevue_ShouldReturnBadRequest()
        {
            var request = new RestRequest("/Revue/Edit?revueId=999999", Method.Put);
            AddAuthHeader(request);

            request.AddJsonBody(new RevueDTO
            {
                Title = "Does not exist",
                Description = "nope",
                Url = ""
            });

            var response = client?.Execute<ApiResponseDTO>(request);

            Assert.That(response?.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.NotFound));

            Assert.That(response?.Content, Is.Not.Null.And.Not.Empty);
        }

        [Test, Order(7)]
        public void DeleteNonExistingRevue_ShouldReturnBadRequest()
        {
            var request = new RestRequest("/Revue/Delete?revueId=00000000-0000-0000-0000-000000000000", Method.Delete);
            AddAuthHeader(request);

            var response = client?.Execute<ApiResponseDTO>(request);

            Assert.That(response?.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.NotFound));

            Assert.That(response?.Content, Is.Not.Null.And.Not.Empty);
        }
    }
}