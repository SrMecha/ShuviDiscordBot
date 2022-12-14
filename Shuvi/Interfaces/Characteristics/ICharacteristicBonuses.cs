using Shuvi.Enums;

namespace Shuvi.Interfaces.Characteristics
{
    public interface ICharacteristicBonuses : ICharacteristics
    {
        public IEnumerator<KeyValuePair<Characteristic, int>> GetEnumerator();
        public void Sum(ICharacteristics bonuses);
        public void Clear();
    }
}
