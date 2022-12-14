using Shuvi.Classes.Characteristics;
using Shuvi.Interfaces.Effects;
using Shuvi.Interfaces.Entity;

namespace Shuvi.Classes.Effects
{
    public class Effects : IEffects
    {
        public List<IEffect> All { get; private set; }

        public Effects()
        {
            All = new List<IEffect>();
        }
        public void Add(IEffect effect)
        {
            All.Add(effect);
        }
        public void Remove(int index)
        {
            if (0 < index && index <= All.Count)
                All.RemoveAt(index);
        }
        public void UpdateAll(IEntity target)
        {
            var bonuses = target.EffectBonuses;
            bonuses.Clear();
            for (int i = All.Count - 1; i >= 0; i--)
            {
                bonuses.Sum(All[i].Bonuses);
                All[i].Update(target);
                if (All[i].TimeLeft <= 0)
                    Remove(i);
            }

        }
    }
}
