using Discord;
using MongoDB.Bson;

namespace Shuvi.Interfaces.Inventory
{
    public interface IUserInventory : IInventoryBase
    {
        public List<SelectMenuOptionBuilder> GetItemsSelectMenu(int index);
        public Embed GetItemsEmbed(int index);
        public Embed GetItemEmbed(ObjectId id);
        public void AddItems(IDropInventory drop);
        public Dictionary<string, int> GetInvetoryCache();
        public int GetTotalEmbeds();
    }
}
