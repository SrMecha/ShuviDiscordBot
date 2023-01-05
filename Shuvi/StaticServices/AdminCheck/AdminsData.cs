using MongoDB.Bson.Serialization.Attributes;

namespace Shuvi.StaticServices.AdminCheck
{
    public class AdminsData
    {
        [BsonId]
        public string Id { get; set; } = "Admins";
        public List<ulong> AdminIds { get; set; } = new();
    }
}
