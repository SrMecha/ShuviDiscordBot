using MongoDB.Bson;
using Shuvi.Classes.Rates;
using Shuvi.Enums;

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
    }
}