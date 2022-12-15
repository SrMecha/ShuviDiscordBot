using Shuvi.Interfaces.Entity;

namespace Shuvi.Interfaces.Effect
{
    public interface IEffects
    {
        public List<IEffect> All { get; }

        public void UpdateAll(IEntity target);
        public void Add(IEffect effect);
        public void Remove(int index);
    }
}
