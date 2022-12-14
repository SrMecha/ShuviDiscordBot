using Shuvi.Interfaces.Characteristics;
using Shuvi.Interfaces.Entity;

namespace Shuvi.Interfaces.Effects
{
    public interface IEffect
    {
        public string Name { get; }
        public int TimeLeft { get; }
        public ICharacteristicBonuses Bonuses { get; }
        public void Update(IEntity target);
    }
}
