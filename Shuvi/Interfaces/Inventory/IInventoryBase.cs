using MongoDB.Bson;
using Shuvi.Interfaces.Items;

namespace Shuvi.Interfaces.Inventory
{
    public interface IInventoryBase
    {
        public int Count { get; }
        public void AddItem(IItem item);
        public void AddItem(ObjectId id, int amount);
        public void RemoveItem(ObjectId id);
        public TItem GetItem<TItem>(ObjectId id) where TItem : IItem;
        public IEnumerable<TItem> GetItems<TItem>() where TItem : IItem;
        public IEnumerable<ObjectId> GetItemsId();
    }
}
