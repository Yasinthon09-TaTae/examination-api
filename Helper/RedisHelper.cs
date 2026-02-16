using Examination.Model;
using System.Text.Json;
using StackExchange.Redis;

namespace Examination.Helper
{
    public class RedisHelper
    {
        private readonly IDatabase _db;
        private const string KEY = "Examinations";

        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        public RedisHelper(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        public async Task SaveAsync(List<Question> data)
        {
            var json = JsonSerializer.Serialize(data);
            await _db.StringSetAsync(KEY, json, TimeSpan.FromMinutes(30));
        }

        public async Task<List<Question>?> LoadAsync()
        {
            var val = await _db.StringGetAsync(KEY);
            if (val.IsNullOrEmpty) return null;

            return JsonSerializer.Deserialize<List<Question>>(val);
        }

        public async Task ClearAsync()
        {
            await _db.KeyDeleteAsync(KEY);
        }
    }
}
