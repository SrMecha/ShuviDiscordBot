using Shuvi.Classes.Status;
using Shuvi.Interfaces.Entity;
using Shuvi.Interfaces.Skills;
using Shuvi.Interfaces.Status;

namespace Shuvi.Classes.Skills
{
    public class VoidSkill : ISkill
    {
        public ISkillInfo Info { get; init; } = new SkillInfo("Нету", "Без описания.");
        public bool CanUse => false;

        public string GetStatus(IEntity owner)
        {
            return "**Способность:** Нету";
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
