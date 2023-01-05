using Shuvi.Enums;
using Shuvi.Interfaces.Status.RatingResult;

namespace Shuvi.Interfaces.User
{
    public interface IUserRating
    {
        public int Points { get; }
        public Rank Rank { get; }
        public Rank GetRank(int rating);
        public IGetRatingResult AddPoints(int amount, Rank rank);
        public IGetRatingResult AddPoints(int amount);
        public IRemoveRatingResult RemovePoints(int amount);
        public ISetRatingResult SetPoints(int amount);
    }
}
