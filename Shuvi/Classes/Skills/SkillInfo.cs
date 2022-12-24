using Shuvi.Interfaces.Skills;

namespace Shuvi.Classes.Skills
{
    public class SkillInfo : ISkillInfo
    {
        public string Name { get; init; }
        public string Description { get; init; }

        public SkillInfo(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
