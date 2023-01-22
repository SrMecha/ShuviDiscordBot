using Shuvi.Enums;
using Shuvi.Interfaces.User;

namespace Shuvi.Classes.User
{
    public class UserWallet : IUserWallet
    {
        public int Money { get; private set; }
        public int Dispoints { get; private set; }

        public UserWallet(int money, int dispoints)
        {
            Money = money;
            Dispoints = dispoints;
        }
        public UserWallet()
        {
            Money = 0;
            Dispoints = 0;
        }
        public void Remove(MoneyType type, int amount)
        {
            switch (type)
            {
                case MoneyType.Simple:
                    Money -= amount;
                    break;
                case MoneyType.Dispoints:
                    Dispoints -= amount;
                    break;
            }
        }
        public void Add(MoneyType type, int amount)
        {
            switch (type)
            {
                case MoneyType.Simple:
                    Money += amount;
                    break;
                case MoneyType.Dispoints:
                    Dispoints += amount;
                    break;
            }
        }
        public void Set(MoneyType type, int amount)
        {
            switch (type)
            {
                case MoneyType.Simple:
                    Money = amount; 
                    break;
                case MoneyType.Dispoints:
                    Dispoints = amount;
                    break;
            }
        }
        public int Get(MoneyType type)
        {
            return type switch
            {
                MoneyType.Simple => Money,
                MoneyType.Dispoints => Dispoints,
                _ => throw new NotImplementedException()
            };
        }
        public void Join(IUserWallet wallet)
        {
            Money += wallet.Money;
            Dispoints += wallet.Dispoints;
        }
    }
}
