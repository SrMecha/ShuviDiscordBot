using MongoDB.Bson;

namespace Shuvi.Classes.Enemy
{
    public class EnemyDrop
    {
        public int Max { get; set; } = 1;
        public int Min { get; set; } = 1;
        public Dictionary<ObjectId, int> Items { get; set; } = new();
    }
}
