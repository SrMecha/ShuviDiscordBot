using Shuvi.Classes.Status;
using Shuvi.Interfaces.Entity;
using Shuvi.Interfaces.Skills;
using Shuvi.Interfaces.Status;

namespace Shuvi.Classes.Skills
{
    public class SkillBase : ISkill
    {
        public virtual ISkillInfo Info { get; init; } = new SkillInfo("Без способности", "Без описания.");
        public virtual bool CanUse => false;

        public virtual string GetStatus(IEntity owner)
        {
            return $"**Способность:** {Info.Name}";
        }
        public virtual void Update(IEntity owner)
        {

        }
        public virtual IActionResult UseSkill(IEntity owner, IEntity target)
        {
            return new ActionResult("Ничего не произошло.");
        }
    }
}
