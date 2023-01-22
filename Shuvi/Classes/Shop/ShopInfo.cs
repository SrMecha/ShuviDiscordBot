using Shuvi.Interfaces.Shop;

namespace Shuvi.Classes.Shop
{
    public class ShopInfo : IShopInfo
    {
        public string Name { get; private set; }
        public string Description { get; private set; }

        public ShopInfo(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
