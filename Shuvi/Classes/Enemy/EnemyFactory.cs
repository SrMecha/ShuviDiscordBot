using MongoDB.Bson;
using Shuvi.Interfaces.Entity;

namespace Shuvi.Classes.Enemy
{
    public static class EnemyFactory
    {
        public static IEnemy GetEnemy(ObjectId id, string picture)
        {
            return new EnemyBase(AllEnemiesData.GetData(id), picture);
        }
    }
}
