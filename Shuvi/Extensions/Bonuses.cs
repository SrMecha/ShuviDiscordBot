using ShuviBot.Enums.Characteristics;
using ShuviBot.Extensions.Items;

namespace ShuviBot.Extensions.Bonuses
{
    public class UserBonuses
    {
        private int _strength;
        private int _agility;
        private int _luck;
        private int _intellect;
        private int _endurance;

        public UserBonuses(List<EquipmentItem> items)
        {
            Dictionary<Characteristics, int> permBonuses = new();
            foreach (EquipmentItem item in items)
                foreach (var (key, value) in item.Bonuses)
                    if (permBonuses.TryAdd(key, value))
                        permBonuses[key] += value;
            _strength = permBonuses.GetValueOrDefault(Characteristics.Strange, 0);
            _agility = permBonuses.GetValueOrDefault(Characteristics.Agility, 0);
            _luck = permBonuses.GetValueOrDefault(Characteristics.Luck, 0);
            _intellect = permBonuses.GetValueOrDefault(Characteristics.Intellect, 0);
            _endurance = permBonuses.GetValueOrDefault(Characteristics.Endurance, 0);
        }

        public int Strength => _strength;
        public int Agility => _agility;
        public int Luck => _luck;
        public int Intellect => _intellect;
        public int Endurance => _endurance;
    }
}