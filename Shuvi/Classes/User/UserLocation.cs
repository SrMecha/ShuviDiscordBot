using Shuvi.Interfaces.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shuvi.Classes.User
{
    public class UserLocation : IUserLocation
    {
        public int MapLocation { get; init; }
        public int MapRegion { get; init; }

        public UserLocation(int mapLocation, int mapRegion)
        {
            MapLocation = mapLocation;
            MapRegion = mapRegion;
        }
    }
}
