using MongoDB.Bson;
using Shuvi.Classes.Interactions;
using Shuvi.Interfaces.User;

namespace Shuvi.Interfaces.Shop
{
    public interface ISellingPart : IShopPartBase
    {
        public bool CanSell(int page, int arrowIndex, CustomContext context, int amount = 1);
        public void Sell(int page, int arrowIndex, CustomContext context, int amount = 1);
    }
}
