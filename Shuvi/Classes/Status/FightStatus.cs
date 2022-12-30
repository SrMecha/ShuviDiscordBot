using Shuvi.Interfaces.Status;

namespace Shuvi.Classes.Status
{
    public class FightStatus : IFightStatus
    {
        public List<string> _descriptions;

        public int Hod { get; set; } = 1;

        public FightStatus()
        {
            _descriptions = new();
        }
        public void AddDescription(string description)
        {
            _descriptions.Add(description);
        }
        public void ClearDescriptions()
        {
            _descriptions.Clear();
        }
        public string GetDescriptions()
        {
            return string.Join("\n", _descriptions);
        }
    }
}
