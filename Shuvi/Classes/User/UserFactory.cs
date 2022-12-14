using Shuvi.Enums;

namespace Shuvi.Classes.User
{
    public static class UserFactory
    {
        public static UserRaces GenerateRandomUserRace()
        {
            Array values = Enum.GetValues(typeof(UserRaces));
            UserRaces? randomRaceOrNull = (UserRaces?)values.GetValue(new Random().Next(values.Length));
            UserRaces randomRace = randomRaceOrNull ?? UserRaces.ExMachina;
            return randomRace;

        }
    }
}
