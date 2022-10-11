using MongoDB.Bson;
using ShuviBot.Enums.Characteristics;
using ShuviBot.Enums.ItemNeeds;
using ShuviBot.Enums.ItemType;
using ShuviBot.Enums.Ranks;
using ShuviBot.Extensions.MongoDocuments;
using ShuviBot.Interfaces.Item;

namespace ShuviBot.Extensions.Items
{
    public class BaseItem : IBaseItem
    {
        protected readonly ObjectId _id;
        protected readonly string _name;
        protected readonly string _description;
        protected readonly ItemType _type;
        protected readonly Ranks _rank;
        protected readonly bool _canTrade;
        protected readonly int _max;
        protected readonly int _amount;

        public BaseItem(ItemDocument data, int amount)
        {
            _id = data.Id;
            _name = data.Name;
            _description = data.Description;
            _type = data.Type;
            _rank = data.Rank;
            _canTrade = data.CanTrade;
            _max = data.Max;
            _amount = amount;
        }
        public ObjectId Id
        {
            get { return _id; }
        }
        public int Amount
        {
            get { return _amount; }
        }
        public string Name
        {
            get { return _name; }
        }
        public string Description
        {
            get { return _description; }
        }
        public int Max
        {
            get { return _max; }
        }
        public virtual string GetBonusesInfo()
        {
            return "���� �������.";
        }
        public virtual string GetNeedsInfo()
        {
            return "���� ����������.";
        }
    }

    public sealed class EquipmentItem : BaseItem, IItem
    {
        private readonly Dictionary<Characteristics, int> _bonuses;
        private readonly Dictionary<ItemNeeds, int> _needs;

        public EquipmentItem(ItemDocument data, int amount) : base(data, amount)
        {
            _bonuses = data.Bonuses;
            _needs = data.Needs;
        }

        public override string GetBonusesInfo()
        {
            string result = "";
            foreach (KeyValuePair<Characteristics, int> bonus in _bonuses)
            {
                result += $"\n{bonus.Key.ToRusString()} +{bonus.Value}";
            }
            if (result == "")
            {
                result = "���� �������.";
            }
            return result;
        }

        public override string GetNeedsInfo()
        {
            string result = "";
            foreach (KeyValuePair<ItemNeeds, int> need in _needs)
            {
                result += $"\n{need.Key.ToRusString()} {need.Value}+";
            }
                if (result == "")
                {
                result = "���� ����������.";
                }
            return result;
        }
    }

    public sealed class SimpleItem : BaseItem, IItem
    {

        public SimpleItem(ItemDocument data, int amount) : base(data, amount)
        {

        }
    }

    public sealed class UsefullItem : BaseItem, IItem
    {

        public UsefullItem(ItemDocument data, int amount) : base(data, amount)
        {

        }
    }



    public sealed class ItemFactory
    {
        public static IItem CreateItem(ItemDocument itemDocument, int amount)
        {
            return itemDocument.Type switch
            {
                ItemType.Weapon => new EquipmentItem(itemDocument, amount),
                ItemType.Helmet => new EquipmentItem(itemDocument, amount),
                ItemType.Armor => new EquipmentItem(itemDocument, amount),
                ItemType.Leggings => new EquipmentItem(itemDocument, amount),
                ItemType.Boots => new EquipmentItem(itemDocument, amount),
                ItemType.Simple => new SimpleItem(itemDocument, amount),
                ItemType.Potion => new UsefullItem(itemDocument, amount),
                ItemType.Chest => new UsefullItem(itemDocument, amount),
                _ => new SimpleItem(itemDocument, amount)
            };
        }
    }
}