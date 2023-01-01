using Shuvi.Interfaces.Inventory;
using Shuvi.Interfaces.Rates;
using Shuvi.Interfaces.Status;

namespace Shuvi.Interfaces.Entity
{
    public interface IEnemy : IEntity
    {
        public int RatingGet { get; init; }
        public string PictureUrl { get; init; }
        public IRate Drop { get; init; }
        public IActionResult RandomAction(IEntity target);
    }
}
