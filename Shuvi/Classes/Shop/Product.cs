using MongoDB.Bson;
using Shuvi.Classes.Items;
using Shuvi.Enums;
using Shuvi.Interfaces.Items;
using Shuvi.Interfaces.Shop;

namespace Shuvi.Classes.Shop
{
    public class Product : IProduct
    {
        public ObjectId Id { get; private set; }
        public string Name { get; private set; }
        public MoneyType MoneyType { get; private set; }
        public int Price { get; private set; }
        public int Amount { get; private set; }

        public Product(ObjectId id, string name, MoneyType moneyType, int price, int amount)
        {
            Id = id;
            MoneyType = moneyType;
            Price = price;
            Amount = amount;
            Name = name;
        }
        public Product(ProductData data, string name)
        {
            Id = data.Id;
            MoneyType = data.Type;
            Price = data.Price;
            Amount = data.Amount;
            Name = name;
        }

        public IItem GetItem()
        {
            return ItemFactory.CreateItem(Id, Amount);
        }
    }
}
