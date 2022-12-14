using Shuvi.Interfaces.Characteristics;
using Shuvi.Enums;
using Shuvi.Interfaces.Equipment;
using Shuvi.Classes.Items;
using Shuvi.Classes.Equipment;
using MongoDB.Bson;

namespace Shuvi.Classes.Characteristics
{
    public class CharacteristicBonuses : ICharacteristicBonuses
    {
        public int Strength { get; set; } = 0;
        public int Agility { get; set; } = 0;
        public int Luck { get; set; } = 0;
        public int Intellect { get; set; } = 0;
        public int Endurance { get; set; } = 0;

        public CharacteristicBonuses(IEquipment items)
        {
            foreach (var item in (UserEquipment)items)
                if (item != null)
                    Sum(AllItemsData.GetItemData((ObjectId)item).Bonuses);
        }
        public CharacteristicBonuses()
        {

        }
        public IEnumerator<KeyValuePair<Characteristic, int>> GetEnumerator()
        {
            if (Strength != 0) 
                yield return new KeyValuePair<Characteristic, int>(Characteristic.Strength, Strength);
            if (Agility != 0)
                yield return new KeyValuePair<Characteristic, int>(Characteristic.Agility, Agility);
            if (Luck != 0)
                yield return new KeyValuePair<Characteristic, int>(Characteristic.Luck, Luck);
            if (Intellect != 0)
                yield return new KeyValuePair<Characteristic, int>(Characteristic.Intellect, Intellect);
            if (Endurance != 0)
                yield return new KeyValuePair<Characteristic, int>(Characteristic.Endurance, Endurance);
        }
        public void Sum(ICharacteristics bonuses)
        {
            Strength = bonuses.Strength;
            Agility = bonuses.Agility;
            Luck = bonuses.Luck;
            Intellect = bonuses.Intellect;
            Endurance = bonuses.Endurance;
        }
        public void Clear()
        {
            Strength = 0;
            Agility = 0;
            Luck = 0;
            Intellect = 0;
            Endurance = 0;
        }
    }
}
