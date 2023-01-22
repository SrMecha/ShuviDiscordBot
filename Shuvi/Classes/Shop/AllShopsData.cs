using MongoDB.Bson;
using Shuvi.Interfaces.Shop;

namespace Shuvi.Classes.Shop
{
    public static class AllShopsData
    {
        private static Dictionary<ObjectId, ShopData> _shops = new();

        public static void LoadShops(Dictionary<ObjectId, ShopData> shops)
        {
            _shops = shops;
        }
        public static List<ShopData> GetShopsData(List<ObjectId> shopIds)
        {
            var result = new List<ShopData>();
            foreach (var (id, data) in _shops)
                if (shopIds.Contains(id))
                    result.Add(data);
            return result;
        }
        public static ShopData GetShopData(ObjectId id)
        {
            return _shops.GetValueOrDefault(id, new());
        }
        public static IShop GetShop(ObjectId id)
        {
            return new GameShop(_shops.GetValueOrDefault(id, new()));
        }
    }
}
