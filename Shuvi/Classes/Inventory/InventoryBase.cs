using Discord;
using MongoDB.Bson;
using Shuvi.Classes.Items;
using Shuvi.Interfaces.Inventory;
using Shuvi.Interfaces.Items;

namespace Shuvi.Classes.Inventory
{
    public class InventoryBase : IInventoryBase
    {
        protected readonly Dictionary<ObjectId, int> _localInventory;

        public int Count => _localInventory.Count;

        public InventoryBase(Dictionary<ObjectId, int> localInventory)
        {
            _localInventory = localInventory;
        }
        public void AddItem(IItem item)
        {
            if (_localInventory.ContainsKey(item.Id))
                _localInventory[item.Id] += item.Amount;
            else
                _localInventory.Add(item.Id, item.Amount);
        }
        public void AddItem(ObjectId id, int amount)
        {
            if (_localInventory.ContainsKey(id))
                _localInventory[id] += amount;
            else
                _localInventory.Add(id, amount);
        }
        public TItem GetItem<TItem>(ObjectId id) where TItem : IItem
        {
            return (TItem)ItemFactory.CreateItem(AllItemsData.GetItemData(id), _localInventory.GetValueOrDefault(id, 0));
        }
        public IItem GetItemAt(int index)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<TItem> GetItems<TItem>() where TItem : IItem
        {
            foreach (var (itemId, amount) in _localInventory)
                yield return (TItem)ItemFactory.CreateItem(AllItemsData.GetItemData(itemId), amount);
        }
        public IEnumerable<ObjectId> GetItemsId()
        {
            return _localInventory.Keys;
        }
        public void RemoveItem(ObjectId id)
        {
            _localInventory.Remove(id);
        }
    }
}
