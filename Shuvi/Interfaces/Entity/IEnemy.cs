using Shuvi.Interfaces.Inventory;

namespace Shuvi.Interfaces.Entity
{
    public interface IEnemy : IEntity
    {
        public string PictureUrl { get; }
        public IDropInventory Drop { get; }
    }
}
