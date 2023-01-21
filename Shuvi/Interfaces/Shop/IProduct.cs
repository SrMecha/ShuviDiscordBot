using MongoDB.Bson;
using Shuvi.Enums;
using Shuvi.Interfaces.Items;

namespace Shuvi.Interfaces.Shop
{
    public interface IProduct
    {
        public ObjectId Id { get; }
        public string Name { get; }
        public MoneyType MoneyType { get; }
        public int Price { get; }
        public int Amount { get; }
        public IItem GetItem();
    }
}
