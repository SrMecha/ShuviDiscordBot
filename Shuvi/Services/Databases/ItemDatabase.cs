using MongoDB.Bson;
using MongoDB.Driver;
using Shuvi.Classes.Items;

namespace Shuvi.Services.Databases
{
    public sealed class ItemDatabase
    {
        private readonly IMongoCollection<ItemData> _itemCollection;

        public ItemDatabase(IMongoCollection<ItemData> itemCollection)
        {
            _itemCollection = itemCollection;
            LoadAllItems();
        }

        private void LoadAllItems()
        {
            foreach (ItemData itemData in _itemCollection.FindSync(new BsonDocument { }).ToEnumerable<ItemData>())
            {
                AllItemsData.AddItemData(itemData);
            }
        }
    }
}