using MongoDB.Bson;
using Shuvi.Enums;

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
    }
}