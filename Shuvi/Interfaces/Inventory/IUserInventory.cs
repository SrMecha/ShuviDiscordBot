using Discord;
using MongoDB.Bson;
using Shuvi.Interfaces.Equipment;
using Shuvi.Interfaces.Items;
using Shuvi.Interfaces.Shop;

namespace Shuvi.Interfaces.Inventory
{
    public interface IUserInventory : IInventoryBase
    {
        public List<SelectMenuOptionBuilder> GetItemsSelectMenu(int index);
        public Embed GetItemsEmbed(int index);
        public Embed GetItemsEmbed(int index, IEquipment equipment);
        public void AddItems(IDropInventory drop);
        public void AddItems(IShopBasket shopBasket);
        public bool HaveItem(ObjectId id, int amount);
        public Dictionary<string, int> GetInvetoryCache();
        public int GetTotalEmbeds();
        public void Clear();
    }
}
