using HackerNewsApi.Models;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace HackerNewsApi.Services
{
    public interface IHackerNewsService
    {
        Task<IEnumerable<int>> GetNewStoryIdsAsync();
        Task<Story> GetStoryByIdAsync(int id);
    }
    public class HackerNewsService : IHackerNewsService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;

        public HackerNewsService(HttpClient httpClient,IMemoryCache cache)
        {
            _cache = cache;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://hacker-news.firebaseio.com/v0/");
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
