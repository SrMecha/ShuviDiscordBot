using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Shuvi.Classes.Shop
{
    public sealed class ShopData
    {
        [BsonId]
        public ObjectId Id { get; set; } = ObjectId.Empty;
        public string Name { get; set; } = "Икея";
        public string Description { get; set; } = "Ебаная икея.";
        public List<ProductData> Sale { get; set; } = new();
        public List<ProductData> Buy { get; set; } = new();
    }
}