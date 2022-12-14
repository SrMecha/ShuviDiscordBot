using Discord;
using MongoDB.Bson;
using Shuvi.Classes.CustomEmbeds;
using Shuvi.Classes.Items;
using Shuvi.Extensions;
using Shuvi.Interfaces.Inventory;
using Shuvi.Interfaces.Items;

namespace Shuvi.Classes.Inventory
{
    public class UserInventory : IUserInventory
    {
        private readonly Dictionary<ObjectId, int> _localInventory;

        public int Count { get => _localInventory.Count; }

        public UserInventory(Dictionary<ObjectId, int> inventoryData)
        {
            _localInventory = inventoryData;
        }
        public void AddItem(IItem item)
        {
            _localInventory.Add(item.Id, item.Amount);
        }
        public IItem GetItem(ObjectId id)
        {
            return ItemFactory.CreateItem(AllItemsData.GetItemData(id), _localInventory.GetValueOrDefault(id, 0));
        }
        public void RemoveItem(ObjectId id)
        {
            _localInventory.Remove(id);
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
                itemsString += $"\n**#{i + 1}** {AllItemsData.GetItemData(_localInventory.Keys.ElementAt(i)).Name} ";
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

        public void AddItem(ObjectId id, int amount)
        {
            _localInventory.Add(id, amount);
        }

        public TItem GetItem<TItem>(ObjectId id) where TItem : IItem
        {
            return (TItem)ItemFactory.CreateItem(AllItemsData.GetItemData(id), _localInventory.GetValueOrDefault(id, 0));
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
    }
}
