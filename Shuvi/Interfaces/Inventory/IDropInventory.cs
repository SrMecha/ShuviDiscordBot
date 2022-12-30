using MongoDB.Bson;

namespace Shuvi.Interfaces.Inventory
{
    public interface IDropInventory
    {
        public string GetDropInfo();
        public IEnumerator<KeyValuePair<ObjectId, int>> GetEnumerator();
    }
}
