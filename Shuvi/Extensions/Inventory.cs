using Discord;
using MongoDB.Bson;
using ShuviBot.Interfaces.Item;
using ShuviBot.Extensions.Items;
using ShuviBot.Extensions.MongoDocuments;
using System.Collections.Immutable;

namespace ShuviBot.Extensions.Inventory
{
    public struct UserInventory
    {
        private Dictionary<ObjectId, int> _localInventory;
        private AllItemsData _itemsConfig;

        public UserInventory(Dictionary<ObjectId, int> inventoryData, AllItemsData itemsConfig)
        {
            _localInventory = inventoryData;
            _itemsConfig = itemsConfig;
        }

        public void AddItem(IItem item)
        {
            _localInventory.Add(item.GetId, item.GetAmount);
        }

        public IItem GetItem(ObjectId id)
        {
            throw new NotImplementedException();
        }

        public void RemoveItem(ObjectId id)
        {
            throw new NotImplementedException();
        }
    }

    public class AllItemsData
    {
        private Dictionary<ObjectId, ItemDocument> _itemsData;
        private List<ItemDocument> _itemsDataArray;

        public AllItemsData () 
        { 
            _itemsData = new Dictionary<ObjectId, ItemDocument>();
            _itemsDataArray = new List<ItemDocument>();
        }

        public void AddItemData(ItemDocument itemData)
        {
            _itemsData.Add(itemData.Id, itemData);
            _itemsDataArray.Add(itemData);
        }

        public ItemDocument GetItemData(ObjectId id)
        {
            ItemDocument? itemData;
            bool itemExist = _itemsData.TryGetValue(id, out itemData);
            if (!itemExist || itemData is null)
                throw new NotImplementedException();
            return itemData;
        }

        public void RemoveItemData(ObjectId id)
        {
            throw new NotImplementedException();
        }

        public List<SelectMenuOptionBuilder> GetItemsSelectMenu(int index)
        {
            List<SelectMenuOptionBuilder> result = new();
            for (int i = index * 10; i <= _itemsDataArray.Count - 1; i++)
            {
                string itemDescription = _itemsDataArray[i].Description;
                if (itemDescription.Length > 70)
                {
                    itemDescription = $"{itemDescription[..70]}...";
                }
                result.Add(new SelectMenuOptionBuilder(
                    _itemsDataArray[i].Name, 
                    _itemsDataArray[i].Id.ToString(),
                    itemDescription
                    ));
             }
            if (result.Count < 1)
            {
                result.Add(new SelectMenuOptionBuilder("None", "None", "Nahuia ti bota slomal"));
            }
            return result;
        }

        public Embed GetItemsEmbed(int index)
        {
            string itemsString = "";
            for(int i = index*10; i<=_itemsDataArray.Count-1; i++)
            {
                itemsString += $"\n**#{i+1}** {_itemsDataArray[i].Name} ";
            }
            return new EmbedBuilder()
                    .WithAuthor("Все предметы:")
                    .WithDescription(itemsString)
                    .WithFooter($"Страница {index + 1}/{(_itemsDataArray.Count + 9) / 10}")
                    .Build();
        }

        public Embed GetItemEmbed(ObjectId id) 
        {
            IItem item = ItemFactory.CreateItem(_itemsData.GetValueOrDefault(id, new ItemDocument()), 0);
            return new EmbedBuilder()
                    .WithAuthor(item.GetName)
                    .WithDescription(item.GetDescription)
                    .WithFooter(item.GetId.ToString())
                    .Build();
        }

        public int GetTotalEmbeds()
        {
            return (_itemsDataArray.Count + 9) / 10;
        }
    }
}