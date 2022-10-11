using MongoDB.Bson;
using ShuviBot.Enums.Ranks;
using ShuviBot.Enums.UserRaces;
using ShuviBot.Extensions.Inventory;
using ShuviBot.Extensions.MongoDocuments;

namespace ShuviBot.Extensions.User
{
    public class User
    {
        private readonly UserInventory _inventory;
        private readonly Ranks _rank;

        public User(UserDocument userData, AllItemsData itemsConfig)
        {
            _inventory = new UserInventory(userData.Inventory, itemsConfig);
        }

        public static UserRaces GenerateRandomUserRace()
        {
            Array values = Enum.GetValues(typeof(UserRaces));
            UserRaces? randomRaceOrNull = (UserRaces?)values.GetValue(new Random().Next(values.Length));
            UserRaces randomRace = randomRaceOrNull ?? UserRaces.ExMachina;
            return randomRace;

        }
    }
}