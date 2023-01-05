using MongoDB.Bson;
using Shuvi.Classes.Items;
using Shuvi.Interfaces.Inventory;
using Shuvi.Interfaces.Items;

namespace Shuvi.Classes.Inventory
{
    public sealed class DropInventory : InventoryBase, IDropInventory
    {
        public Dictionary<ObjectId, int> Items => _localInventory;

        public DropInventory(Dictionary<ObjectId, int> inventoryData) : base(inventoryData)
        {

        }
        public DropInventory() : base(new())
        {

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
