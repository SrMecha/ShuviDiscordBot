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
            var toAdd = 0;
            if (item.Max == -1)
                toAdd = item.Amount;
            else
            {
                var alreadyHave = _localInventory.GetValueOrDefault(item.Id, 0);
                if (alreadyHave + item.Amount > item.Max)
                    toAdd = item.Max - alreadyHave;
                else
                    toAdd = item.Amount;
            }
            if (_localInventory.ContainsKey(item.Id))
                _localInventory[item.Id] += toAdd;
            else
                _localInventory.Add(item.Id, toAdd);
        }
        public void AddItem(ObjectId id, int amount)
        {
            AddItem(ItemFactory.CreateItem(id, amount));
        }
        public TItem GetItem<TItem>(ObjectId id) where TItem : IItem
        {
            return (TItem)ItemFactory.CreateItem(AllItemsData.GetItemData(id), _localInventory.GetValueOrDefault(id, 0));
        }
        public IItem GetItem(ObjectId id)
        {
            return ItemFactory.CreateItem(AllItemsData.GetItemData(id), _localInventory.GetValueOrDefault(id, 0));
        }
        public IItem GetItemAt(int index)
        {
            var (id, amount) = _localInventory.ElementAt(index);
            return ItemFactory.CreateItem(id, amount);
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
        public void RemoveItem(ObjectId id, int amount)
        {
            if (!_localInventory.ContainsKey(id))
                return;
            _localInventory[id] -= amount;
            if (_localInventory[id] < 1)
                _localInventory.Remove(id);
        }
    }
}
