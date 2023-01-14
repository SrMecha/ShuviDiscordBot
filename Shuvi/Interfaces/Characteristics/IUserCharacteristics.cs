namespace Shuvi.Interfaces.Characteristics
{
    public interface IUserCharacteristics : ICharacteristics
    {
        public string ToRusString(ICharacteristics bonuses);
        public string ToRusString(ICharacteristics bonuses, int intellect);
        public void Add(ICharacteristics characteristics);
        public void Reset();
    }
}
