using Discord;
using MongoDB.Bson;
using Shuvi.Classes.CustomEmbeds;
using Shuvi.Classes.Items;
using Shuvi.Classes.Shop;
using Shuvi.Interfaces.Inventory;
using Shuvi.Interfaces.Items;
using Shuvi.Interfaces.Shop;

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
            foreach (var (id, amount) in drop)
            {
                AddItem(id, amount);
            }
        }
        public void AddItems(IShopBasket shopBasket)
        {
            foreach (var (product, amount) in shopBasket)
            {
                if (_localInventory.GetValueOrDefault(product.Id, 0) + amount == 0)
                    _localInventory.Remove(product.Id);
                else
                    AddItem(product.Id, amount);
            }
        }
        public List<SelectMenuOptionBuilder> GetItemsSelectMenu(int index)
        {
            var result = new List<SelectMenuOptionBuilder>();
            for (int i = index * 10; i < _localInventory.Count && i < index * 10 + 10; i++)
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
            for (int i = index * 10; i < _localInventory.Count && i < index * 10 + 10; i++)
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
        public int GetTotalEmbeds()
        {
            return ((_localInventory.Count + 9) / 10) < 1 ? 1 : (_localInventory.Count + 9) / 10;
        }
        public bool HaveItem(ObjectId id, int amount)
        {
            if (!_localInventory.ContainsKey(id))
                return false;
            return _localInventory.GetValueOrDefault(id, 0) >= amount;
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
                if (GetItemAt(i).CanLoose)
                    RemoveItem(_localInventory.Keys.ElementAt(i));
        }
    }
}
