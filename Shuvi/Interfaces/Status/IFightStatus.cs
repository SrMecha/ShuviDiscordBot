namespace Shuvi.Interfaces.Status
{
    public interface IFightStatus
    {
        public int Hod { get; set; }
        public void AddDescription(string description);
        public void ClearDescriptions();
        public string GetDescriptions();
    }
}
