using MongoDB.Bson.Serialization.Attributes;

namespace Shuvi.StaticServices.Logs
{
    public class LogsData
    {
        [BsonId]
        public string Id { get; set; } = "Logs";
        public ulong ServerLogChannelId { get; set; } = 0;
    }
}
