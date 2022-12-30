using MongoDB.Bson;

namespace Shuvi.Interfaces.Rates
{
    public interface IIdRate
    {
        public ObjectId GetRandom();
    }
}
