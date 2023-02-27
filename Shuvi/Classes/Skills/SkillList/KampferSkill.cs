using Shuvi.Classes.Characteristics;
using Shuvi.Classes.Effect;
using Shuvi.Classes.Status;
using Shuvi.Interfaces.Entity;
using Shuvi.Interfaces.Skills;
using Shuvi.Interfaces.Status;

namespace Shuvi.Classes.Skills.SkillList
{
    public class KampferSkill : SkillBase
    {

        public override ISkillInfo Info { get; init; } = new SkillInfo("Боевой режим", "Увеличение выносливости и силы на 20%. Действует продолжительное время.");
        public override bool CanUse => UsesLeft > 0;
        public int UsesLeft { get; private set; } = 1;

        public override string GetStatus(IEntity onwer)
        {
            return $"**Способность:** {Info.Name}\n**Использований осталось:** {UsesLeft}";
        }
        public override IActionResult UseSkill(IEntity owner, IEntity target)
        {
            UsesLeft--;
            owner.Effects.Add(new EffectBase(Info.Name, 20, new CharacteristicBonuses()
            { 
                Strength = (int)(owner.Characteristics.Strength / 100.0f * 20),
                Endurance = (int)(owner.Characteristics.Endurance / 100.0f * 20)
            }
            ));
            return new ActionResult($"{owner.Name} активировал навык「Боевой режим」.");
        }
    }
}
