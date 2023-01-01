using Shuvi.Enums;
using Shuvi.Extensions;
using Shuvi.Interfaces.Status;
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
                >= 5000 => Rank.SSS,
                >= 3500 => Rank.SS,
                >= 2000 => Rank.S,
                >= 1000 => Rank.A,
                >= 600  => Rank.B,
                >= 300  => Rank.C,
                >= 100 => Rank.D,
                _ => Rank.E
            };
        }
    }
}
