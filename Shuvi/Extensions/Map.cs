using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ShuviBot.Enums.Ranks;

namespace ShuviBot.Extensions.Map
{
    public class WorldMap
    {
        [BsonElement("Settings")]
        public MapSettings Settings { get; set; } = new();
        [BsonElement("Regions")]
        public List<MapRegion> Regions { get; set; } = new();
    }

    public class MapSettings
    {
        public string PictureURL { get; set; } = "https://i.imgur.com/otCYNya.jpg";
    }

    public class MapRegion
    {
        public string Name { get; set; } = "NoName";
        public string Description { get; set; } = "NoDescription";
        public Rank NeededRank { get; set; } = Rank.E;
        public List<RegionLocation> Locations { get; set; } = new();
        public List<ObjectId> Shops { get; set; } = new();
        public List<ObjectId> Dungeons { get; set; } = new();  
        public string PictureURL { get; set; } = "https://i.imgur.com/otCYNya.jpg";
    }

    public class RegionLocation
    {
        public string Name { get; set; } = "NoName";
        public string Description { get; set; } = "NoDescription";
        public Rank RecomendedRank { get; set; } = Rank.E;
        public List<ObjectId> Enemies { get; set; } = new();
        public List<>
        public string PictureURL { get; set; } = "https://i.imgur.com/otCYNya.jpg";
    }
}