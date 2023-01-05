using Shuvi.Interfaces.User;

namespace Shuvi.Classes.User
{
    public class UserLocation : IUserLocation
    {
        public int MapLocation { get; private set; }
        public int MapRegion { get; private set; }

        public UserLocation(int mapLocation, int mapRegion)
        {
            MapLocation = mapLocation;
            MapRegion = mapRegion;
        }
        public void SetLocation(int index)
        {
            MapLocation = index;
        }
        public void SetRegion(int index)
        {
            MapRegion = index;
        }
    }
}
