using Shuvi.Classes.Status;
using Shuvi.Interfaces.Entity;
using Shuvi.Interfaces.Skills;
using Shuvi.Interfaces.Status;

namespace Shuvi.Classes.Skills.SkillList
{
    public class VoidSkill : ISkill
    {
        public ISkillInfo Info { get; init; } = new SkillInfo("Без способности", "Без описания.");
        public bool CanUse => false;

        public string GetStatus(IEntity owner)
        {
            return $"**Способность:** {Info.Name}";
        }
        public void Update(IEntity owner)
        {

        }
        public IActionResult UseSkill(IEntity owner, IEntity target)
        {
            return new ActionResult("Ничего не произошло.");
        }
    }
}
