using Shuvi.Interfaces.Inventory;
using Shuvi.Interfaces.Skills;
using Shuvi.Interfaces.Status;

namespace Shuvi.Interfaces.Entity
{
    public interface IPlayer : IEntity
    {
        public ISkill Skill { get; init; }
        public IUserInventory Inventory { get; init; }
        public IActionResult UseSkill(IEntity target);
    }
}
