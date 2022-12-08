using Discord;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ShuviBot.Enums.Ranks;
using ShuviBot.Extensions.Rates;

namespace ShuviBot.Extensions.Map
{
    public class WorldMap
    {
        [BsonId]
        public string Id { get; set; } = string.Empty;
        public MapSettings Settings { get; set; } = new();
        public List<MapRegion> Regions { get; set; } = new();

        public void Configure()
        {
            foreach (var region in Regions)
                foreach (var location in region.Locations)
                    location.ConfigureRates();
        }
        public MapRegion GetRegion(int index)
        {
            if (Regions.Count - 1 > index)
                return new MapRegion { };
            return Regions[index];
        }
        public List<SelectMenuOptionBuilder> GetRegionsSelectMenu()
        {
            List<SelectMenuOptionBuilder> result = new();
            for (int i = 0; i <= Regions.Count - 1; i++)
            {
                MapRegion region = Regions[i];
                string regionDescription = region.Description;
                if (regionDescription.Length > 70)
                {
                    regionDescription = $"{regionDescription[..70]}...";
                }
                result.Add(new SelectMenuOptionBuilder(
                    region.Name,
                    i.ToString(),
                    regionDescription
                    ));
            }
            if (result.Count < 1)
            {
                result.Add(new SelectMenuOptionBuilder("None", "None", "Nahuia ti bota slomal"));
            }
            return result;
        }
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
        public Rank RecomendedRank { get; set; } = Rank.E;
        public List<RegionLocation> Locations { get; set; } = new();
        public List<ObjectId> Shops { get; set; } = new();
        public List<ObjectId> Dungeons { get; set; } = new();
        public string PictureURL { get; set; } = "https://i.imgur.com/otCYNya.jpg";

        public RegionLocation GetLocation(int index)
        {
            if (Locations.Count - 1 > index)
                return new RegionLocation { };
            return Locations[index];
        }
    }

    public class RegionLocation
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