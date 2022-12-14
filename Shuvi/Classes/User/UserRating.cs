using Shuvi.Enums;
using Shuvi.Interfaces.User;

namespace Shuvi.Classes.User
{
    public class UserRating : IUserRating
    {
        public int Points { get; init; }
        public Rank Rank { get; init; }

        public UserRating(int rating)
        {
            Points = rating;
            Rank = GetRank(rating);
        }
        public Rank GetRank(int rating)
        {
            return rating switch
            {
                < 100 => 0,
                >= 100 and < 200 => (Rank)1,
                >= 200 and < 500 => (Rank)2,
                >= 500 and < 800 => (Rank)3,
                >= 800 and < 1100 => (Rank)4,
                >= 1100 and < 1500 => (Rank)5,
                >= 1500 and < 2000 => (Rank)6,
                >= 2000 => (Rank)7
            };
        }
    }
}
