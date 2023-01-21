using MongoDB.Bson;
using Shuvi.Enums;

namespace Shuvi.Classes.Shop
{
    public class ProductData
    {
        public ObjectId Id { get; set; } = ObjectId.Empty;
        public MoneyType Type { get; set; } = MoneyType.Simple;
        public int Price { get; set; } = 0;
        public int Amount { get; set; } = 1;
    }
}
