using Discord;
using MongoDB.Bson;
using Shuvi.Classes.Items;
using Shuvi.Classes.Rates;
using Shuvi.Classes.Shop;
using Shuvi.Enums;
using Shuvi.Interfaces.Shop;

namespace Shuvi.Classes.Map
{
    public class MapLocation
    {
        public string Name { get; set; } = "NoName";
        public string Description { get; set; } = "NoDescription";
        public Rank RecomendedRank { get; set; } = Rank.E;
        public Dictionary<ObjectId, int> Enemies { get; set; } = new();
        public EnemyRate EnemiesWithChance { get; set; } = new();
        public List<DropData> MineDrop { get; set; } = new();
        public EachRate MineDropWithChance { get; set; } = new();
        public List<ObjectId> Shops { get; set; } = new();
        public List<ObjectId> Dungeons { get; set; } = new();
        public string PictureURL { get; set; } = "https://i.imgur.com/otCYNya.jpg";

        public void ConfigureRates()
        {
            EnemiesWithChance = new EnemyRate(Enemies);
            MineDropWithChance = new EachRate(MineDrop);
        }
        public List<ShopData> GetShopsData()
        {
            return AllShopsData.GetShopsData(Shops);
        }
        public IShop GetShop(ObjectId id)
        {
            return AllShopsData.GetShop(id);
        }
        public List<SelectMenuOptionBuilder> GetShopsSelectMenu()
        {
            var result = new List<SelectMenuOptionBuilder>();
            foreach(var shop in GetShopsData())
            {
                var shopDescription = shop.Description;
                if (shopDescription.Length > 70)
                {
                    shopDescription = $"{shopDescription[..70]}...";
                }
                result.Add(new SelectMenuOptionBuilder(
                    shop.Name,
                    shop.Id.ToString(),
                    shopDescription
                    ));
            }
            if (result.Count < 1)
            {
                result.Add(new SelectMenuOptionBuilder("None", "None", "Nahuia ti bota slomal"));
            }
            return result;
        }
    }
}