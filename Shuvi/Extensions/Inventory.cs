using Discord;
using MongoDB.Bson;
using ShuviBot.Interfaces.Item;
using ShuviBot.Extensions.Items;
using ShuviBot.Extensions.MongoDocuments;
using System.Collections.Immutable;
using ShuviBot.Enums.ItemType;
using ShuviBot.Enums.Ranks;

namespace ShuviBot.Extensions.Inventory
{
    public struct UserInventory
    {
        private readonly Dictionary<ObjectId, int> _localInventory;
        private readonly AllItemsData _itemsConfig;

        public UserInventory(Dictionary<ObjectId, int> inventoryData, AllItemsData itemsConfig)
        {
            _localInventory = inventoryData;
            _itemsConfig = itemsConfig;
        }
        public int Count
        {
            get => _localInventory.Count;
        }

        public void AddItem(IItem item)
        {
            _localInventory.Add(item.Id, item.Amount);
        }
        public IItem GetItem(ObjectId id)
        {
            return ItemFactory.CreateItem(_itemsConfig.GetItemData(id), _localInventory.GetValueOrDefault(id, 0));
        }
        public void RemoveItem(ObjectId id)
        {
            _localInventory.Remove(id);
        }
        public List<SelectMenuOptionBuilder> GetItemsSelectMenu(int index)
        {
            List<SelectMenuOptionBuilder> result = new();
            for (int i = index * 10; i <= _localInventory.Count - 1; i++)
            {
                ItemDocument item = _itemsConfig.GetItemData(_localInventory.Keys.ElementAt(i));
                string itemDescription = item.Description;
                if (itemDescription.Length > 70)
                {
                    itemDescription = $"{itemDescription[..70]}...";
                }
                result.Add(new SelectMenuOptionBuilder(
                    item.Name,
                    item.Id.ToString(),
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
            for (int i = index * 10; i <= _localInventory.Count - 1; i++)
            {
                itemsString += $"\n**#{i + 1}** {_itemsConfig.GetItemData(_localInventory.Keys.ElementAt(i)).Name} ";
            }
            if (itemsString == "") itemsString = "У вас нету предметов.";
            return new EmbedBuilder()
                    .WithAuthor("Все предметы:")
                    .WithDescription(itemsString)
                    .WithFooter($"Страница {index + 1}/{GetTotalEmbeds()}")
                    .Build();
        }
        public Embed GetItemEmbed(ObjectId id)
        {
            IItem item = ItemFactory.CreateItem(_itemsConfig.GetItemData(id), _localInventory.GetValueOrDefault(id, 0));
            string bonuses = "";
            string needs = "";
            if (item.Type.WithBonuses())
            {
                bonuses = $"**Бонусы:**\n{item.GetBonusesInfo()}";
                needs = $"**Требования:**\n{item.GetNeedsInfo()}";
            }

            return new EmbedBuilder()
                    .WithAuthor("Просмотр предмета")
                    .WithDescription($"**Название:** {item.Name}\n**Тип:** {item.Type.ToRusString()}\n" +
                    $"**Ранг:** {item.Rank.ToRusString()}\n**Максимум в инвентаре:** {(item.Max < 0 ? "бесконечно" : item.Max)}\n**У вас есть:** {item.Amount}\n\n" +
                    $"**Описание:**\n{item.Description}\n{(item.CanTrade ? "Можно обменять" : "Нельзя обменять")}\n\n{bonuses}\n{needs}")
                    .WithFooter($"ID: {item.Id}")
                    .WithColor(item.Rank.GetColor())
                    .Build();
        }
        public int GetTotalEmbeds()
        {
            return ((_localInventory.Count + 9) / 10) < 1 ? 1 : (_localInventory.Count + 9) / 10;
        }
    }

    public class AllItemsData
    {
        private readonly Dictionary<ObjectId, ItemDocument> _itemsData;
        private readonly List<ItemDocument> _itemsDataArray;

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
            string bonuses = "";
            string needs = "";
            if (item.Type.WithBonuses())
            {
                bonuses = $"**Бонусы:**\n{item.GetBonusesInfo()}";
                needs = $"**Требования:**\n{item.GetNeedsInfo()}";
            }

            return new EmbedBuilder()
                    .WithAuthor("Просмотр предмета")
                    .WithDescription($"**Название:** {item.Name}\n**Тип:** {item.Type.ToRusString()}\n" +
                    $"**Ранг:** {item.Rank.ToRusString()}\n**Максимум в инвентаре:** {(item.Max < 0 ? "бесконечно" : item.Max)}\n\n" +
                    $"**Описание:**\n{item.Description}\n{(item.CanTrade ? "Можно обменять" : "Нельзя обменять")}\n\n{bonuses}\n{needs}")
                    .WithFooter($"ID: {item.Id}")
                    .WithColor(item.Rank.GetColor())
                    .Build();
        }

        public int GetTotalEmbeds()
        {
            return (_itemsDataArray.Count + 9) / 10;
        }
    }
}