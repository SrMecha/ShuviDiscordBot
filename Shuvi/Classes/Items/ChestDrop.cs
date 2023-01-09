using MongoDB.Bson;
using Shuvi.Classes.Rates;

namespace Shuvi.Classes.Items
{
    public class ChestDrop
    {
        public int DispointsMax { get; set; } = 0;
        public int DispointsMin { get; set; } = 0;
        public int ItemsMin { get; set; } = 0;
        public int ItemsMax { get; set; } = 0;
        public Dictionary<ObjectId, int> Items { get; set; } = new();
    }
}
