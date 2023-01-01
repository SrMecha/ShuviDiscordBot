﻿using Shuvi.Enums;
using Shuvi.Interfaces.Status;

namespace Shuvi.Classes.Status
{
    public class GetRatingResult : ActionResult, IGetRatingResult
    {
        public bool IsRankUp { get; init; }
        public Rank RankBefore { get; init; }
        public Rank RankAfter { get; init; }

        public GetRatingResult(Rank rankBefore, Rank rankAfter, string description) : base(description)
        {
            RankBefore = rankBefore;
            RankAfter = rankAfter;
            if (RankBefore < RankAfter)
                IsRankUp = true;
            else
                IsRankUp = false;
        }
    }
}
