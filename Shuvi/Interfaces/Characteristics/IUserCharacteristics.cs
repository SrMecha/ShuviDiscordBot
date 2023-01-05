namespace Shuvi.Interfaces.Characteristics
{
    public interface IUserCharacteristics : ICharacteristics
    {
        public string ToRusString(ICharacteristics bonuses);
        public void Reset();
    }
}
