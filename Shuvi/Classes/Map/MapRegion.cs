using Discord;
using MongoDB.Bson;
using Shuvi.Classes.Items;
using Shuvi.Enums;
using Shuvi.Extensions;
using System;

namespace Shuvi.Classes.Map
{
    public class MapRegion
    {
        public string Name { get; set; } = "NoName";
        public string Description { get; set; } = "NoDescription";
        public Rank NeededRank { get; set; } = Rank.E;
        public Rank RecomendedRank { get; set; } = Rank.E;
        public List<MapLocation> Locations { get; set; } = new();
        public List<ObjectId> Shops { get; set; } = new();
        public List<ObjectId> Dungeons { get; set; } = new();
        public string PictureURL { get; set; } = "https://i.imgur.com/otCYNya.jpg";

        public MapLocation GetLocation(int index)
        {
            if (Locations.Count - 1 > index)
                return new MapLocation { };
            return Locations[index];
        }
        public List<SelectMenuOptionBuilder> GetLocationsSelectMenu()
        {
            var result = new List<SelectMenuOptionBuilder>();
            for (int i = 0; i < Locations.Count; i++)
            {
                var location = GetLocation(i);
                var description = location.Description;
                if (description.Length > 70)
                {
                    description = $"{description[..70]}...";
                }
                result.Add(new SelectMenuOptionBuilder(
                    $"{location.Name} 「{location.RecomendedRank.ToRusString()}」",
                    i.ToString(),
                    description
                    ));
            }
            if (result.Count < 1)
            {
                result.Add(new SelectMenuOptionBuilder("None", "None", "В данном регионе нету локаций."));
            }
            return result;
        }
    }
}