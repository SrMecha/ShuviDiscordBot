using MongoDB.Bson;
using Shuvi.Classes.Interactions;
using Shuvi.Interfaces.User;

namespace Shuvi.Interfaces.Shop
{
    public interface IPurchasingPart : IShopPartBase
    {
        public bool CanBuy(int page, int arrowIndex, CustomContext context, int amount = 1);
        public void Buy(int page, int arrowIndex, CustomContext context, int amount = 1);
    }
}
