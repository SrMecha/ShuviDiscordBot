using Discord;
using MongoDB.Bson;
using Shuvi.Extensions;
using Shuvi.Interfaces.Items;
using Shuvi.Classes.CustomEmbeds;

namespace Shuvi.Classes.Items
{
    public static class AllItemsData
    {
        private static readonly Dictionary<ObjectId, ItemData> _itemsData = new();
        private static readonly List<ItemData> _itemsDataArray = new();

        public static void Init()
        {

        }
        public static void AddItemData(ItemData itemData)
        {
            _itemsData.Add(itemData.Id, itemData);
            _itemsDataArray.Add(itemData);
        }

        public static ItemData GetItemData(ObjectId id)
        {
            if (_itemsData.TryGetValue(id, out var itemData))
                return itemData;
            return new ItemData();
        }

        public static void RemoveItemData(ObjectId id)
        {
            _itemsDataArray.Remove(GetItemData(id));
            _itemsData.Remove(id);
        }

        public static List<SelectMenuOptionBuilder> GetItemsSelectMenu(int index)
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

        public static Embed GetItemsEmbed(int index)
        {
            string itemsString = "";
            for (int i = index * 10; i <= _itemsDataArray.Count - 1; i++)
            {
                itemsString += $"\n**#{i + 1}** {_itemsDataArray[i].Name} ";
            }
            return new BotEmbedBuilder()
                    .WithAuthor("Все предметы:")
                    .WithDescription(itemsString)
                    .WithFooter($"Страница {index + 1}/{(_itemsDataArray.Count + 9) / 10}")
                    .Build();
        }

        public static Embed GetItemEmbed(ObjectId id)
        {
            IItem item = ItemFactory.CreateItem(_itemsData.GetValueOrDefault(id, new ItemData()), 0);
            string bonuses = "";
            string needs = "";
            if (item.Type.WithBonuses())
            {
                bonuses = $"**Бонусы:**\n{item.GetBonusesInfo()}";
                needs = $"**Требования:**\n{item.GetNeedsInfo()}";
            }

            return new BotEmbedBuilder()
                    .WithAuthor("Просмотр предмета")
                    .WithDescription($"**Название:** {item.Name}\n**Тип:** {item.Type.ToRusString()}\n" +
                    $"**Ранг:** {item.Rank.ToRusString()}\n**Максимум в инвентаре:** {(item.Max < 0 ? "бесконечно" : item.Max)}\n\n" +
                    $"**Описание:**\n{item.Description}\n{(item.CanTrade ? "Можно обменять" : "Нельзя обменять")}\n\n{bonuses}\n{needs}")
                    .WithFooter($"ID: {item.Id}")
                    .WithColor(item.Rank.GetColor())
                    .Build();
        }

        public static int GetTotalEmbeds()
        {
            return (_itemsDataArray.Count + 9) / 10;
        }
    }
}
