using HackerNewsApi.Controllers;
using HackerNewsApi.Models;
using HackerNewsApi.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace HackerNewsApi.Classes
{
    public class StoriesControllerTests
    {
        private readonly Mock<IHackerNewsService> _mockService;
        private readonly StoriesController _controller;

        public StoriesControllerTests()
        {
            _mockService = new Mock<IHackerNewsService>();
            _controller = new StoriesController(_mockService.Object);
        }

        [Fact]
        public async Task GetNewStories_ReturnsOkResult()
        {
            // Arrange
            var storyIds = new List<int> { 1, 2, 3 };
            var stories = new List<Story>
        {
            new Story { Id = 1, Title = "Story 1", Url = "http://example.com/1" },
            new Story { Id = 2, Title = "Story 2", Url = "http://example.com/2" }
        };

            _mockService.Setup(service => service.GetNewStoryIdsAsync()).ReturnsAsync(storyIds);
            _mockService.Setup(service => service.GetStoryByIdAsync(It.IsAny<int>())).ReturnsAsync((int id) => stories.Find(s => s.Id == id));

            // Act
            var result = await _controller.GetNewStories();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedStories = Assert.IsType<List<Story>>(okResult.Value);
            Assert.Equal(2, returnedStories.Count);
        }

        [Fact]
        public async Task SearchStories_ReturnsOkResult()
        {
            // Arrange
            var storyIds = new List<int> { 1, 2, 3 };
            var stories = new List<Story>
        {
            new Story { Id = 1, Title = "Story 1", Url = "http://example.com/1" },
            new Story { Id = 2, Title = "Story 2", Url = "http://example.com/2" }
        };

            _mockService.Setup(service => service.GetNewStoryIdsAsync()).ReturnsAsync(storyIds);
            _mockService.Setup(service => service.GetStoryByIdAsync(It.IsAny<int>())).ReturnsAsync((int id) => stories.Find(s => s.Id == id));

            // Act
            var result = await _controller.SearchStories("Story");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedStories = Assert.IsType<List<Story>>(okResult.Value);
            Assert.Equal(2, returnedStories.Count);
        }
    }
}
