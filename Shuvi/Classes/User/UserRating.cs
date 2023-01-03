using Shuvi.Classes.Status.RatingResult;
using Shuvi.Enums;
using Shuvi.Extensions;
using Shuvi.Interfaces.Status.RatingResult;
using Shuvi.Interfaces.User;

namespace Shuvi.Classes.User
{
    public class UserRating : IUserRating
    {
        public int Points { get; private set; }
        public Rank Rank { get; private set; }

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
        public IGetRatingResult AddPoints(int amount, Rank rank)
        {
            if (Rank < rank)
                return new GetRatingResult(Rank, Rank, "Ранг побежденного существа слишком мало для полечения очков рейтинга.");
            Points += amount;
            var rankBefore = Rank;
            Rank = GetRank(Points);
            if (rankBefore < Rank)
                return new GetRatingResult(rankBefore, Rank, $"Вы получили {amount} очков рейтинга, и повысили свой ранг до {Rank.ToRusString()}!");
            return new GetRatingResult(rankBefore, Rank, $"Вы получили {amount} очков рейтинга.");
            
        }
        public IGetRatingResult AddPoints(int amount)
        {
            Points += amount;
            var rankBefore = Rank;
            Rank = GetRank(Points);
            if (rankBefore < Rank)
                return new GetRatingResult(rankBefore, Rank, $"Вы получили {amount} очков рейтинга, и повысили свой ранг до {Rank.ToRusString()}!");
            return new GetRatingResult(rankBefore, Rank, $"Вы получили {amount} очков рейтинга.");

        }
        public IRemoveRatingResult RemovePoints(int amount)
        {
            Points -= amount;
            var rankBefore = Rank;
            Rank = GetRank(Points);
            if (rankBefore > Rank)
                return new RemoveRatingResult(rankBefore, Rank, $"Вы потеряли {amount} очков рейтинга, и понизили свой ранг до {Rank.ToRusString()}.");
            return new RemoveRatingResult(rankBefore, Rank, $"Вы потеряли {amount} очков рейтинга.");
        }
        public ISetRatingResult SetPoints(int amount)
        {
            Points = amount;
            var rankBefore = Rank;
            Rank = GetRank(Points);
            if (rankBefore != Rank)
                return new SetRatingResult(rankBefore, Rank, $"Вам присвоено {amount} очков рейтинга. Вы получили ранг {Rank.ToRusString()}.");
            return new SetRatingResult(rankBefore, Rank, $"Вам присвоено {amount} очков рейтинга.");
        }
    }
}
