using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MySuperStats.Contracts.Requests;
using MySuperStats.Contracts.Responses;
using MySuperStats.WebApi.ApplicationSettings;
using MySuperStats.WebApi.Business;
using MySuperStats.WebApi.Models;
using System.Collections.Generic;
using MySuperStats.Contracts.Enums;
using System;
using CustomFramework.BaseWebApi.Identity.Controllers;
using CustomFramework.BaseWebApi.Authorization.Attributes;
using CustomFramework.BaseWebApi.Authorization.Enums;
using CustomFramework.BaseWebApi.Utils.Utils.Exceptions;
using CustomFramework.BaseWebApi.Utils.Contracts;
using CustomFramework.BaseWebApi.Resources;

namespace MySuperStats.WebApi.Controllers
{
    [Route(ApiConstants.DefaultRoute + "MatchGroup")]
    public class MatchGroupController : BaseControllerWithCrudAuthorization<MatchGroup, MatchGroupRequest, MatchGroupRequest, MatchGroupResponse, IMatchGroupManager, int>
    {
        private readonly IPermissionChecker _permissionChecker;
        private readonly IMatchGroupUserManager _matchGroupUserManager;
        public MatchGroupController(IPermissionChecker permissionChecker, IMatchGroupUserManager matchGroupUserManager, ILocalizationService localizationService, ILogger<Controller> logger, IMapper mapper, IMatchGroupManager manager)
         : base(localizationService, logger, mapper, manager)
        {
            _matchGroupUserManager = matchGroupUserManager;
            _permissionChecker = permissionChecker;
        }

        [Route("create")]
        [HttpPost]
        [Permission(nameof(PermissionEnum.CreateMatchGroup), nameof(BooleanEnum.True))]
        public Task<IActionResult> Create([FromBody]MatchGroupRequest request)
        {
            return CommonOperationAsync<IActionResult>(async () =>
            {
                if (!ModelState.IsValid)
                    throw new ArgumentException(ModelState.ModelStateToString(LocalizationService));

                var result = await Manager.CreateAsync(request);

                return Ok(new ApiResponse(LocalizationService, Logger).Ok(
                    Mapper.Map<MatchGroup, MatchGroupResponse>(result)));
            });
        }

        [Route("{id:int}/update")]
        [HttpPut]
        public async Task<IActionResult> Update(int id, [FromBody]MatchGroupRequest request)
        {
            var attributes = new List<PermissionAttribute> {
                    new PermissionAttribute(nameof(PermissionEnum.UpdateMatchGroup), nameof(BooleanEnum.True))
                 };
            await _permissionChecker.HasPermissionAsync(User, id, attributes);

            return await BaseUpdateAsync(id, request);
        }

        [Route("delete/{id:int}")]
        [HttpDelete]
        [Permission(nameof(PermissionEnum.DeleteMatchGroup), nameof(BooleanEnum.True))]
        public async Task<IActionResult> Delete(int id)
        {
            return await BaseDeleteAsync(id);
        }

        [Route("get/id/{id:int}")]
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            return await BaseGetByIdAsync(id);
        }

        [Route("get/matchId/{matchId:int}")]
        [HttpGet]
        public Task<IActionResult> GetByGroupName(int matchId)
        {
            return CommonOperationAsync<IActionResult>(async () =>
            {
                var result = await Manager.GetByMatchIdAsync(matchId);

                return Ok(new ApiResponse(LocalizationService, Logger).Ok(
                    Mapper.Map<MatchGroup, MatchGroupResponse>(result)));
            });
        }        

        [Route("get/groupname/{groupName}")]
        [HttpGet]
        public Task<IActionResult> GetByGroupName(string groupName)
        {
            return CommonOperationAsync<IActionResult>(async () =>
            {
                var result = await Manager.GetByGroupNameAsync(groupName);

                return Ok(new ApiResponse(LocalizationService, Logger).Ok(
                    Mapper.Map<MatchGroup, MatchGroupResponse>(result)));
            });
        }
    }
}