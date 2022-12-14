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
        public UserRaces Race { get; init; }
        public UserProfessions Profession { get; init; }
        public IUserInventory Inventory { get; init; }
        public IEquipment Equipment { get; init; }
        public ICharacteristics Characteristic { get; init; }
        public IEnergy Energy { get; init; }
        public IMana Mana { get; init; }
        public IHealth Health { get; init; }
        public IUserStatistics Statistics { get; init; }
        public IUserLocation Location { get; init; }
    }
}
