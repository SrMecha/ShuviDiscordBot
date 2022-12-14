using Shuvi.Interfaces.Status;

namespace Shuvi.Classes.Status
{
    public class ActionResult : IActionResult
    {
        public string Description { get; private set; }

        public ActionResult(string description)
        {
            Description = description;
        }
    }

}
