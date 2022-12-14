using Shuvi.Interfaces.User;

namespace Shuvi.Classes.User
{
    public class UserWallet : IUserWallet
    {
        public int Money { get; init; }
        public int Dispoints { get; init; }

        public UserWallet(int money, int dispoints)
        {
            Money = money;
            Dispoints = dispoints;
        }
    }
}
