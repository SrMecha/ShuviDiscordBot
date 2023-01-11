using Shuvi.Interfaces.Status;
using Shuvi.Interfaces.User;

namespace Shuvi.Interfaces.Items
{
    public interface IUsefullItem : IItem
    {
        public Task<IActionResult> Use(IDatabaseUser dbUser);
    }
}
