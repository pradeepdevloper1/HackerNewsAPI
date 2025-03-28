using HackerNewsApi.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace HackerNewsApi.Services
{
    public interface IHackerNewsService
    {
        Task<IEnumerable<int>> GetNewStoryIdsAsync();
        Task<Story> GetStoryByIdAsync(int id);
        Task<IEnumerable<Story>> GetTopStoriesAsync(int count);
    }
    public class HackerNewsService : IHackerNewsService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<HackerNewsService> _logger;

        private const string CacheKey = "TopStoriesResponse";


        public HackerNewsService(HttpClient httpClient, IMemoryCache cache, ILogger<HackerNewsService> logger)
        {
            _httpClient = httpClient;

            _cache = cache;
            _logger = logger;
            _httpClient.BaseAddress = new Uri("https://hacker-news.firebaseio.com/v0/");
        }
        private async Task<List<Story>> GetStoriesBatchAsync(IEnumerable<int> storyIds)
        {
            var storyTasks = storyIds.Select(id => GetStoryAsync(id));
            var stories = await Task.WhenAll(storyTasks);
            return stories.Where(s => s != null && !string.IsNullOrEmpty(s.Url)).ToList();
        }
        private async Task<Story> GetStoryAsync(int id)
        {
            try
            {
                var response = await _httpClient.GetStringAsync($"item/{id}.json");
                return JsonConvert.DeserializeObject<Story>(response);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"Error fetching story {id}");
                return null;
            }
        }
        public async Task<IEnumerable<Story>> GetTopStoriesAsync(int count)
        {
            try
            {
                return await _cache.GetOrCreateAsync(CacheKey, async entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);

                    var storyIds = await GetNewStoryIdsAsync();
                    var stories = await GetStoriesBatchAsync(storyIds.Take(200));

                    return stories.Take(count);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting top stories");
                throw;
            }
        }
        public async Task<IEnumerable<int>> GetNewStoryIdsAsync()
        {
            const string cacheKey = "NewStoryIds";

            if (!_cache.TryGetValue(cacheKey, out IEnumerable<int> storyIds))
            {
                // Fetch data from the API if not in cache
                var response = await _httpClient.GetStringAsync("newstories.json");
                storyIds = JsonConvert.DeserializeObject<IEnumerable<int>>(response).Take(200);

                // Cache the data for 5 minutes
                _cache.Set(cacheKey, storyIds, TimeSpan.FromMinutes(5));
            }

            return storyIds;
        }

        public async Task<Story> GetStoryByIdAsync(int id)
        {
            var cacheKey = $"Story_{id}";

            // Check if the data is in the cache
            if (!_cache.TryGetValue(cacheKey, out Story story))
            {
                // Fetch data from the API if not in cache
                var response = await _httpClient.GetStringAsync($"item/{id}.json");
                story = JsonConvert.DeserializeObject<Story>(response);

                // Cache the data for 5 minutes
                _cache.Set(cacheKey, story, TimeSpan.FromMinutes(5));
            }

            return story;
        }
    }
}
