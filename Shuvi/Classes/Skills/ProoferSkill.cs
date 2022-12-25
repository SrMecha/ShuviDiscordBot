using Shuvi.Classes.Characteristics;
using Shuvi.Classes.Effect;
using Shuvi.Classes.Status;
using Shuvi.Interfaces.Entity;
using Shuvi.Interfaces.Skills;
using Shuvi.Interfaces.Status;

namespace Shuvi.Classes.Skills
{
    public class ProoferSkill : ISkill
    {
        public ISkillInfo Info { get; init; } = new SkillInfo("Оценка", "Увеличивает интеллект на 30%. Действует несколько ходов.");

        public void Update(IEntity owner)
        {

        }
        public IActionResult UseSkill(IEntity owner, IEntity target)
        {
            owner.Effects.Add(new EffectBase(Info.Name, 3, new CharacteristicBonuses() 
                { Intellect = (int)(owner.Characteristics.Intellect / 100.0f * 30) }
            ));
            return new ActionResult($"{owner.Name} активировал навык「Оценка」."); 
        }
    }
}
