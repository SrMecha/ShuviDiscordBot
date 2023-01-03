using Shuvi.Enums;

namespace Shuvi.Interfaces.Status.RatingResult
{
    public interface ISetRatingResult : IActionResult
    {
        public bool IsRankChanged { get; init; }
        public Rank RankBefore { get; init; }
        public Rank RankAfter { get; init; }
    }
}