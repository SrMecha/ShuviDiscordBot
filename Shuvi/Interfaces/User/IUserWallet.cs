namespace Shuvi.Interfaces.User
{
    public interface IUserWallet
    {
        public int Money { get; }
        public int Dispoints { get; }
        public void SetMoney(int amount);
        public void AddMoney(int amount);
        public void RemoveMoney(int amount);
        public void SetDispoints(int amount);
        public void AddDispoints(int amount);
        public void RemoveDispoints(int amount);
    }
}
