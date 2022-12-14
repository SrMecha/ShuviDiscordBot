using Shuvi.Interfaces.Inventory;

namespace Shuvi.Interfaces.Entity
{
    public interface IPlayer : IEntity
    {
        public IUserInventory Inventory { get; init; }
    }
}
