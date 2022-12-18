using MongoDB.Bson;

namespace Shuvi.Classes.Enemy
{
    public static class AllEnemiesData
    {
        private static Dictionary<ObjectId, EnemyData> Data { get; set; } = new();

        public static void AddData(EnemyData data)
        {
            Data.Add(data.Id, data);
        }
        public static EnemyData GetData(ObjectId id)
        {
            return Data.GetValueOrDefault(id, new());
        }
    }
}
