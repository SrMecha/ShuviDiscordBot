using MongoDB.Bson;

namespace Shuvi.Classes.Rates
{
    public sealed class DropData
    {
        public ObjectId Id { get; set; } = ObjectId.Empty;
        public int Chance { get; set; } = 0;
        public int Min { get; set; } = 1;
        public int Max { get; set; } = 1;
    }
}