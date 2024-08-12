using Microsoft.Extensions.Options;
using MongoDB.Driver;
using UrlShortenerApi.Models;

namespace UrlShortenerApi.Data
{
    public class UrlShortenerContext
    {
        private readonly IMongoDatabase _database;

        public UrlShortenerContext(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<UrlMapping> UrlMappings => _database.GetCollection<UrlMapping>("UrlMappings");
    }
}
