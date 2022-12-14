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
        public AllRate EnemiesWithChance { get; set; } = new();
        public List<DropData> MineDrop { get; set; } = new();
        public string PictureURL { get; set; } = "https://i.imgur.com/otCYNya.jpg";

        public void ConfigureRates()
        {
            EnemiesWithChance = new AllRate(Enemies);
        }
    }
}