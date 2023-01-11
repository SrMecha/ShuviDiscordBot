using Discord;
using MongoDB.Bson;
using Shuvi.Interfaces.Items;

namespace Shuvi.Interfaces.Inventory
{
    public interface IUserInventory : IInventoryBase
    {
        public List<SelectMenuOptionBuilder> GetItemsSelectMenu(int index);
        public Embed GetItemsEmbed(int index);
        public void AddItems(IDropInventory drop);
        public bool HaveItem(ObjectId id, int amount);
        public Dictionary<string, int> GetInvetoryCache();
        public int GetTotalEmbeds();
        public void Clear();
    }
}
