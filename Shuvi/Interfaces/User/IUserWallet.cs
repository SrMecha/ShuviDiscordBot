using Shuvi.Enums;

namespace Shuvi.Interfaces.User
{
    public interface IUserWallet
    {
        public int Money { get; }
        public int Dispoints { get; }
        public void Remove(MoneyType type, int amount);
        public void Add(MoneyType type, int amount);
        public void Set(MoneyType type, int amount);
        public int Get(MoneyType type);
        public void Join(IUserWallet wallet);
    }
}
