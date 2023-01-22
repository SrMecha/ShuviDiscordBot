using Discord;
using MongoDB.Bson.Serialization.Attributes;

namespace Shuvi.Classes.Map
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
            if (index >= Regions.Count)
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
}