using Shuvi.Enums;

namespace Shuvi.Interfaces.Status
{
    public interface IGetRatingResult : IActionResult
    {
        public bool IsRankUp { get; init; }
        public Rank RankBefore { get; init; }
        public Rank RankAfter { get; init; }
    }
}
