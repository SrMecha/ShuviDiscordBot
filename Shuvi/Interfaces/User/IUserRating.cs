using Shuvi.Enums;

namespace Shuvi.Interfaces.User
{
    public interface IUserRating
    {
        public int Points { get; init; }
        public Rank Rank { get; init; }
        public Rank GetRank(int rating);
    }
}
