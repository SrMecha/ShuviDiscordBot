using MongoDB.Bson;
using MongoDB.Driver;
using Shuvi.Classes.Enemy;

namespace Shuvi.Services.Databases
{
    public sealed class EnemiesDatabase
    {
        private readonly IMongoCollection<EnemyData> _enemiesCollection;

        public EnemiesDatabase(IMongoCollection<EnemyData> enemyCollection)
        {
            _enemiesCollection = enemyCollection;
            LoadEnemies();
        }
        private void LoadEnemies()
        {
            foreach (EnemyData enemyData in _enemiesCollection.FindSync(new BsonDocument { }).ToEnumerable<EnemyData>())
            {
                AllEnemiesData.AddData(enemyData);
            }
        }
    }
}