using Shuvi.Enums;
using Shuvi.Interfaces.Status;

namespace Shuvi.Classes.Status
{
    public class RatingResult : ActionResult, IRatingResult
    {
        public bool IsRankChanged { get; init; }
        public Rank RankBefore { get; init; }
        public Rank RankAfter { get; init; }

        public RatingResult(Rank rankBefore, Rank rankAfter, string description) : base(description)
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
