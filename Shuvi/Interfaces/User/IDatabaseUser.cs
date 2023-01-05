using Shuvi.Enums;
using Shuvi.Interfaces.Characteristics;
using Shuvi.Interfaces.Equipment;
using Shuvi.Interfaces.Inventory;

namespace Shuvi.Interfaces.User
{
    public interface IDatabaseUser
    {
        public ulong Id { get; init; }
        public IUserRating Rating { get; init; }
        public IUserWallet Wallet { get; init; }
        public string SpellName { get; }
        public UserRaces Race { get; }
        public UserProfessions Profession { get; }
        public IUserInventory Inventory { get; init; }
        public IEquipment Equipment { get; init; }
        public IUserCharacteristics Characteristic { get; init; }
        public IEnergy Energy { get; init; }
        public IMana Mana { get; init; }
        public IHealth Health { get; init; }
        public IUserStatistics Statistics { get; init; }
        public IUserLocation Location { get; init; }

        public void SetSpell(string name);
        public void SetRace(UserRaces race);
        public void SetProfession(UserProfessions profession);
    }
}
