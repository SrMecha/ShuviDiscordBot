using Shuvi.Classes.Characteristics;
using Shuvi.Enums;
using Shuvi.Extensions;
using Shuvi.Interfaces.Characteristics;

namespace Shuvi.Classes.Items
{
    public sealed class EquipmentItem : BaseItem
    {
        public override ICharacteristicBonuses Bonuses { get; protected set; }
        public override Dictionary<ItemNeeds, int> Needs { get; protected set; }

        public EquipmentItem(ItemData data, int amount) : base(data, amount)
        {
            Bonuses = data.Bonuses;
            Needs = data.Needs;
        }
        public override string GetBonusesInfo()
        {
            var result = "";
            foreach (var bonus in Bonuses)
            {
                result += $"{bonus.Key.ToRusString()} {bonus.Value}\n+";
            }
            if (result == "")
            {
                result = "Нету бонусов.";
            }
            return result;
        }
        public override string GetNeedsInfo()
        {
            var result = "";
            foreach (var need in Needs)
            {
                result += $"{need.Key.ToRusString()} {need.Key.GetFormatString(need.Value)}+\n";
            }
            if (result == "")
            {
                result = "Нету требований.";
            }
            return result;
        }
    }
}