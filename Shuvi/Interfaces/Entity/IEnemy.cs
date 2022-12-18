using Shuvi.Interfaces.Inventory;
using Shuvi.Interfaces.Rates;

namespace Shuvi.Interfaces.Entity
{
    public interface IEnemy : IEntity
    {
        public string PictureUrl { get; init; }
        public IRate Drop { get; init; }
    }
}
