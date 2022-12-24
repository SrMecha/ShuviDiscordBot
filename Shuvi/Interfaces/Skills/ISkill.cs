using Shuvi.Interfaces.Entity;
using Shuvi.Interfaces.Status;

namespace Shuvi.Interfaces.Skills
{
    public interface ISkill
    {
        public ISkillInfo Info { get; init; }
        public IActionResult UseSkill(IEntity owner, IEntity target);
        public void Update(IEntity owner);
    }
}
