using Shuvi.Interfaces.Items;

namespace Shuvi.Classes.Items
{
    public sealed class UsefullItem : BaseItem, IItem
    {

        public UsefullItem(ItemData data, int amount) : base(data, amount)
        {

        }
    }
}