using System.Collections.Generic;
using System.Threading.Tasks;
using CustomFramework.BaseWebApi.Utils.Business;
using MySuperStats.Contracts.Requests;
using MySuperStats.WebApi.Models;

namespace MySuperStats.WebApi.Business
{
    public interface IMatchGroupTeamManager : IBusinessManager<MatchGroupTeam, MatchGroupTeamRequest, int>
    {
        Task<IList<MatchGroup>> GetMatchGroupsByTeamIdAsync(int teamId);
        Task<IList<Team>> GetTeamsByMatchGroupIdAsync(int matchGroupId);
    }
}