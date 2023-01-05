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
        public void AddMoney(int amount)
        {
            Money += amount;
        }
        public void RemoveMoney(int amount)
        {
            Money -= amount;
        }
        public void AddDispoints(int amount)
        {
            Dispoints += amount;
        }
        public void RemoveDispoints(int amount)
        {
            Dispoints -= amount;
        }

        public void SetMoney(int amount)
        {
            Money = amount;
        }

        public void SetDispoints(int amount)
        {
            Dispoints = amount;
        }
    }
}
