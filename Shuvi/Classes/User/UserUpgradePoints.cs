using Shuvi.Classes.Characteristics;
using Shuvi.Interfaces.Characteristics;
using Shuvi.Interfaces.User;

namespace Shuvi.Classes.User
{
    public class UserUpgradePoints : IUserUpgradePoints
    {
        private IUserRating Rating { get; init; }
        private IUserCharacteristics Characteristics { get; init; }
        private IMana Mana { get; init; }
        private IHealth Health { get; init; }

        public UserUpgradePoints(IUserRating rating, IUserCharacteristics characteristics, IMana mana, IHealth health)
        {
            Rating = rating;
            Characteristics = characteristics;
            Mana = mana;
            Health = health;
        }

        public int GetPoints()
        {
            var tempUserData = new UserData();
            var pointsOccupied = 0;
            pointsOccupied += Characteristics.Strength + Characteristics.Agility + 
                Characteristics.Luck + Characteristics.Endurance + Characteristics.Intellect - 5;
            pointsOccupied += (Mana.Max - tempUserData.MaxMana) / CharacteristicSettings.ManaPerUpPoint;
            pointsOccupied += (Health.Max - tempUserData.MaxHealth) / CharacteristicSettings.HealthPerUpPoint;
            return (int)Math.Ceiling((float)Rating.Points / UserSettings.RatingPerUpdgradePoint) - pointsOccupied;
        }
    }
}
