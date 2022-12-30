using MongoDB.Bson;
using Shuvi.Classes.Items;
using Shuvi.Interfaces.Inventory;

namespace Shuvi.Classes.Inventory
{
    public sealed class DropInventory : IDropInventory
    {
        private readonly Dictionary<ObjectId, int> _localInventory;

        public Dictionary<ObjectId, int> Items => _localInventory;

        public DropInventory(Dictionary<ObjectId, int> inventoryData)
        {
            _localInventory = inventoryData;
        }
        public DropInventory()
        {
            _localInventory = new();
        }
        public void AddItem(ObjectId id, int amount = 1)
        {
            if (Items.ContainsKey(id))
                _localInventory[id] += amount;
            else
                _localInventory.Add(id, amount);
        }

        public IEnumerator<KeyValuePair<ObjectId, int>> GetEnumerator()
        {
            return _localInventory.GetEnumerator();
        }

        public string GetDropInfo()
        {
            var result = new List<string>();
            foreach (var (itemId, amount) in _localInventory)
            {
                var item = ItemFactory.CreateItem(AllItemsData.GetItemData(itemId), amount);
                result.Add($"{item.Name} +{amount}");
            }
            if (result.Count == 0)
                return "Ничего";
            return string.Join("\n", result);

        }
    }
}
