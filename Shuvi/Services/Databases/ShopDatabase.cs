using MongoDB.Bson;
using MongoDB.Driver;
using Shuvi.Classes.Shop;

namespace Shuvi.Services.Databases
{
    public sealed class ShopDatabase
    {
        private readonly IMongoCollection<ShopData> _shopCollection;

        public Dictionary<ObjectId, ShopData> Shops { get; init; }

        public ShopDatabase(IMongoCollection<ShopData> shopCollection)
        {
            _shopCollection = shopCollection;
            Shops = LoadShops();
        }
        private Dictionary<ObjectId, ShopData> LoadShops()
        {
            Dictionary<ObjectId, ShopData> result = new();
            foreach (ShopData shopData in _shopCollection.FindSync(new BsonDocument { }).ToEnumerable<ShopData>())
            {
                result.Add(shopData.Id, shopData);
            }
            return result;
        }
    }
}