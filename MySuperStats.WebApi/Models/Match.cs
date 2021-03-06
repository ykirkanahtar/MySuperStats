using CustomFramework.BaseWebApi.Data.Models;
using System;
using System.Collections.Generic;

namespace MySuperStats.WebApi.Models
{
    public class Match : BaseModel<int>
    {
        private DateTime _matchDateTime;

        public DateTime MatchDate
        {
            get => _matchDateTime;
            set => _matchDateTime = value.Date;
        }

        public int MatchGroupId { get; set; }
        public int Order { get; set; }
        public int DurationInMinutes { get; set; }
        public int HomeTeamId { get; set; }
        public int AwayTeamId { get; set; }
        public decimal HomeTeamScore { get; set; }
        public decimal AwayTeamScore { get; set; }
        public string VideoLink { get; set; }

        public virtual MatchGroup MatchGroup { get; set; }
        public virtual Team HomeTeam { get; set; }
        public virtual Team AwayTeam { get; set; }

        public virtual ICollection<BasketballStat> BasketballStats { get; set; }
        public virtual ICollection<FootballStat> FootballStats { get; set; }

    }
}
