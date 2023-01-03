using Shuvi.Enums;
using Shuvi.Interfaces.Status.RatingResult;

namespace Shuvi.Classes.Status.RatingResult
{
    public class SetRatingResult : ActionResult, ISetRatingResult
    {
        public bool IsRankChanged { get; init; }
        public Rank RankBefore { get; init; }
        public Rank RankAfter { get; init; }

        public SetRatingResult(Rank rankBefore, Rank rankAfter, string description) : base(description)
        {
            RankBefore = rankBefore;
            RankAfter = rankAfter;
            if (RankBefore != RankAfter)
                IsRankChanged = true;
            else
                IsRankChanged = false;
        }
    }
}
