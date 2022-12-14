using Shuvi.Classes.Characteristics;
using Shuvi.Interfaces.Characteristics;
using Shuvi.Interfaces.Effects;
using Shuvi.Interfaces.Entity;
namespace Shuvi.Classes.Effects
{
    public class EffectBase : IEffect
    {
        public string Name { get; protected set; }
        public int TimeLeft { get; protected set; }
        public ICharacteristicBonuses Bonuses { get; protected set; }

        public EffectBase(string name, int timeLeft, CharacteristicBonuses? bonuses)
        {
            Name = name;
            TimeLeft = timeLeft;
            Bonuses = bonuses ?? new CharacteristicBonuses();
        }
        public virtual void Update(IEntity target)
        {
            TimeLeft--;
        }
    }
}
