using HackerNewsApi.Models;
using HackerNewsApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HackerNewsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoriesController : ControllerBase
    {
        private readonly IHackerNewsService _hackerNewsService;
        private readonly ILogger<StoriesController> _logger;

        public StoriesController(IHackerNewsService hackerNewsService, ILogger<StoriesController> logger)
        {
            _hackerNewsService = hackerNewsService;
            _logger = logger;
        }

        [HttpGet("new")]
        public async Task<IActionResult> GetNewStories([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var storyIds = await _hackerNewsService.GetNewStoryIdsAsync();
            var paginatedIds = storyIds.Skip((page - 1) * pageSize).Take(pageSize);

            var stories = new List<Story>();
            foreach (var id in paginatedIds)
            {
                var story = await _hackerNewsService.GetStoryByIdAsync(id);
                if (story != null && !string.IsNullOrEmpty(story.Url))
                {
                    stories.Add(story);
                }
            }

            return Ok(stories);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchStories([FromQuery] string query, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var storyIds = await _hackerNewsService.GetNewStoryIdsAsync();
            var stories = new List<Story>();

            foreach (var id in storyIds)
            {
                var story = await _hackerNewsService.GetStoryByIdAsync(id);
                if (story != null && !string.IsNullOrEmpty(story.Url) && story.Title.Contains(query, StringComparison.OrdinalIgnoreCase))
                {
                    stories.Add(story);
                }
            }

            var paginatedStories = stories.Skip((page - 1) * pageSize).Take(pageSize);
            return Ok(paginatedStories);
        }


        [HttpGet("top")]
        [ResponseCache(Duration = 300)] // Client-side caching
        public async Task<IActionResult> GetTopStories([FromQuery] int count = 10)
        {
            try
            {
                if (count <= 0 || count > 100)
                {
                    return BadRequest("Count must be between 1 and 100");
                }

                var stories = await _hackerNewsService.GetTopStoriesAsync(count);
                return Ok(stories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting top stories");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
