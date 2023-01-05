using Shuvi.Enums;
using Shuvi.Interfaces.Status.RatingResult;

namespace Shuvi.Classes.Status.RatingResult
{
    public class RemoveRatingResult : ActionResult, IRemoveRatingResult
    {
        public bool IsRankDown { get; init; }
        public Rank RankBefore { get; init; }
        public Rank RankAfter { get; init; }

        public RemoveRatingResult(Rank rankBefore, Rank rankAfter, string description) : base(description)
        {
            RankBefore = rankBefore;
            RankAfter = rankAfter;
            if (RankBefore < RankAfter)
                IsRankDown = false;
            else
                IsRankDown = true;
        }
    }
}
