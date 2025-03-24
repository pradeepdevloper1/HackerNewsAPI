using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Xunit;
using System.Net.Http;
using System.Threading.Tasks;


namespace HackerNewsApi.Classes
{
    public class StoriesControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public StoriesControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GetNewStories_ReturnsSuccessStatusCode()
        {
            // Act
            var response = await _client.GetAsync("/api/stories/new");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task SearchStories_ReturnsSuccessStatusCode()
        {
            // Act
            var response = await _client.GetAsync("/api/stories/search?query=test");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
