using MySuperStats.WebApi.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using CustomFramework.BaseWebApi.Data.Repositories;

namespace MySuperStats.WebApi.Data.Repositories
{
    public interface IMatchRepository : IRepository<Match, int>
    {
        Task<Match> GetByMatchDateAndOrderAsync(int matchGroupId, DateTime matchDate, int order);
        Task<IList<Match>> GetAllByMatchGroupIdAsync(int matchGroupId);
        Task<IList<Match>> GetMatchForMainScreen(int matchGroupId);
        Task<Match> GetMatchDetailBasketballStats(int matchId);
        Task<Match> GetMatchDetailFootballStats(int matchId);
    }
}