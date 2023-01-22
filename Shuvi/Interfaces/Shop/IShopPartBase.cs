using Discord;
using Shuvi.Classes.Interactions;
using Shuvi.Interfaces.Items;

namespace Shuvi.Interfaces.Shop
{
    public interface IShopPartBase
    {
        public Embed GetEmbed(CustomContext context, int page, int arrowIndex);
        public Embed GetEmbed(CustomContext context, int page);
        public List<SelectMenuOptionBuilder> GetSelectMenuOptions(int page);
        public IItem GetItem(int itemIndex);
        public IItem GetItem(int page, int arrowIndex);
        public bool HaveProducts();
        public int GetTotalEmbeds();
    }
}
