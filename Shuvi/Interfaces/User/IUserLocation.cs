namespace Shuvi.Interfaces.User
{
    public interface IUserLocation
    {
        public int MapLocation { get; }
        public int MapRegion { get; }
        public void SetLocation(int index);
        public void SetRegion(int index);
    }
}
