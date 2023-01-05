using Discord;
using MongoDB.Bson;
using Shuvi.Classes.CustomEmbeds;
using Shuvi.Classes.Items;
using Shuvi.Extensions;
using Shuvi.Interfaces.Inventory;
using Shuvi.Interfaces.Items;
using System.Linq;

namespace Shuvi.Classes.Inventory
{
    public class UserInventory : InventoryBase, IUserInventory
    {
        public UserInventory(Dictionary<ObjectId, int> inventoryData) : base(inventoryData) 
        {

        }
        public UserInventory() : base(new())
        {

        }
        public void AddItems(IDropInventory drop)
        {
            foreach (var item in drop)
            {
                if (_localInventory.ContainsKey(item.Key))
                    _localInventory[item.Key] += item.Value;
                else
                    _localInventory.Add(item.Key, item.Value);
            }
        }
        public IItem GetItem(ObjectId id)
        {
            return ItemFactory.CreateItem(AllItemsData.GetItemData(id), _localInventory.GetValueOrDefault(id, 0));
        }
        public List<SelectMenuOptionBuilder> GetItemsSelectMenu(int index)
        {
            var result = new List<SelectMenuOptionBuilder>();
            for (int i = index * 10; i <= _localInventory.Count - 1; i++)
            {
                var item = AllItemsData.GetItemData(_localInventory.Keys.ElementAt(i));
                var itemDescription = item.Description;
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
            var itemsString = "";
            for (int i = index * 10; i <= _localInventory.Count - 1; i++)
            {
                itemsString += $"\n**#{i + 1}** {AllItemsData.GetItemData(_localInventory.Keys.ElementAt(i)).Name} x{_localInventory.Values.ElementAt(i)}";
            }
            if (itemsString == "") itemsString = "У вас нету предметов.";
            return new BotEmbedBuilder()
                    .WithAuthor("Все предметы:")
                    .WithDescription(itemsString)
                    .WithFooter($"Страница {index + 1}/{GetTotalEmbeds()}")
                    .Build();
        }
        public Embed GetItemEmbed(ObjectId id)
        {
            var item = ItemFactory.CreateItem(AllItemsData.GetItemData(id), _localInventory.GetValueOrDefault(id, 0));
            var bonuses = "";
            var needs = "";
            if (item.Type.WithBonuses())
            {
                bonuses = $"**Бонусы:**\n{item.GetBonusesInfo()}";
                needs = $"**Требования:**\n{item.GetNeedsInfo()}";
            }

            return new BotEmbedBuilder()
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
        public Dictionary<string, int> GetInvetoryCache()
        {
            Dictionary<string, int> result = new();
            foreach (var item in _localInventory)
                result.Add(item.Key.ToString(), item.Value);
            return result;
        }
        public void Clear()
        {
            for (int i = _localInventory.Count - 1; i > 0; i--)
                if (!GetItemAt(i).CanLoose)
                    RemoveItem(_localInventory.Keys.ElementAt(i));
        }
    }
}
