using Shuvi.Interfaces.Items;

namespace Shuvi.Classes.Items
{
    public sealed class SimpleItem : BaseItem, IItem
    {

        public SimpleItem(ItemData data, int amount) : base(data, amount)
        {

        }
    }
}