using MongoDB.Bson;
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
            _rank = (Ranks)data.Rank;
            _canTrade = data.CanTrade;
            _max = data.Max;
            _amount = amount;
        }

        public ObjectId GetId
        {
            get { return _id; }
        }

        public int GetAmount
        {
            get { return _amount; }
        }

        public string GetName
        {
            get { return _name; }
        }

        public string GetDescription
        {
            get { return _description; }
        }

        public int Max
        {
            get { return _max; }
        }
    }

    public sealed class EquipmentItem : BaseItem, IItem
    {
        private readonly Dictionary<string, int> _bonuses;
        private readonly Dictionary<string, int> _needs;

        public EquipmentItem(ItemDocument data, int amount) : base(data, amount)
        {
            _bonuses = data.Bonuses ?? new Dictionary<string, int>();
            _needs = data.Needs ?? new Dictionary<string, int>();
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
            switch (itemDocument.Type)
            {
                case ItemType.Weapon: return new EquipmentItem(itemDocument, amount);
                case ItemType.Helmet: return new EquipmentItem(itemDocument, amount);
                case ItemType.Armor: return new EquipmentItem(itemDocument, amount);
                case ItemType.Leggings: return new EquipmentItem(itemDocument, amount);
                case ItemType.Boots: return new EquipmentItem(itemDocument, amount);
                case ItemType.Simple: return new SimpleItem(itemDocument, amount);
                case ItemType.Potion: return new UsefullItem(itemDocument, amount);
                case ItemType.Chest: return new UsefullItem(itemDocument, amount);
                default: return new SimpleItem(itemDocument, amount);
            }
        }
    }
}