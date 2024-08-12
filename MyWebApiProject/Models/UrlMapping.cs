using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace UrlShortenerApi.Models
{
    public class UrlMapping
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("ShortUrl")]
        public string ShortUrl { get; set; }

        [BsonElement("OriginalUrl")]
        public string OriginalUrl { get; set; }
    }
}
