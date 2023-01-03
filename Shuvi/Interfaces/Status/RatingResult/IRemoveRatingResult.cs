using Shuvi.Enums;

namespace Shuvi.Interfaces.Status.RatingResult
{
    public interface IRemoveRatingResult : IActionResult
    {
        public bool IsRankDown { get; init; }
        public Rank RankBefore { get; init; }
        public Rank RankAfter { get; init; }
    }
}
