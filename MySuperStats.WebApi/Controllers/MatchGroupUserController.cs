using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MySuperStats.Contracts.Requests;
using MySuperStats.Contracts.Responses;
using MySuperStats.WebApi.ApplicationSettings;
using MySuperStats.WebApi.Business;
using MySuperStats.WebApi.Models;
using MySuperStats.Contracts.Enums;
using System;
using CustomFramework.BaseWebApi.Identity.Controllers;
using CustomFramework.BaseWebApi.Resources;
using CustomFramework.BaseWebApi.Authorization.Attributes;
using CustomFramework.BaseWebApi.Authorization.Enums;
using CustomFramework.BaseWebApi.Utils.Utils.Exceptions;
using CustomFramework.BaseWebApi.Utils.Contracts;

namespace MySuperStats.WebApi.Controllers
{
    [Route(ApiConstants.DefaultRoute + "MatchGroupUser")]
    public class MatchGroupUserController : BaseControllerWithCrdAuthorization<MatchGroupUser, MatchGroupPlayerRequest, MatchGroupUserResponse, IMatchGroupUserManager, int>
    {
        private readonly IPermissionChecker _permissionChecker;
        public MatchGroupUserController(IPermissionChecker permissionChecker, ILocalizationService localizationService, ILogger<Controller> logger, IMapper mapper, IMatchGroupUserManager manager)
             : base(localizationService, logger, mapper, manager)
        {
            _permissionChecker = permissionChecker;
        }

        [Route("create")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]MatchGroupPlayerCreateRequest request)
        {
            var attributes = new List<PermissionAttribute> {
                    new PermissionAttribute(nameof(PermissionEnum.CreateMatchGroupUser), nameof(BooleanEnum.True))
                 };
            await _permissionChecker.HasPermissionAsync(User, request.MatchGroupId, attributes);

            var matchGroupPlayerRequest = Mapper.Map<MatchGroupPlayerRequest>(request);
            matchGroupPlayerRequest.RoleId = (int)RoleEnum.Player;
            return await BaseCreateAsync(matchGroupPlayerRequest);
        }

        [Route("updaterole")]
        [HttpPut]
        public Task<IActionResult> UpdateRoleAsync([FromBody]MatchGroupUserRequest request)
        {
            return CommonOperationAsync<IActionResult>(async () =>
            {
                var attributes = new List<PermissionAttribute> {
                    new PermissionAttribute(nameof(PermissionEnum.AddUserToRole), nameof(BooleanEnum.True))
                 };
                await _permissionChecker.HasPermissionAsync(User, request.MatchGroupId, attributes);

                if (!ModelState.IsValid)
                    throw new ArgumentException(ModelState.ModelStateToString(LocalizationService));

                var result = await Manager.UpdateRoleAsync(request);
                return Ok(new ApiResponse(LocalizationService, Logger).Ok(Mapper.Map<MatchGroupUser, MatchGroupUserResponse>(result)));
            });
        }

        [Route("delete/{id:int}")]
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var matchGroupUser = await Manager.GetByIdAsync(id);

            var attributes = new List<PermissionAttribute> {
                    new PermissionAttribute(nameof(PermissionEnum.DeleteMatchGroupUser), nameof(BooleanEnum.True))
                 };
            await _permissionChecker.HasPermissionAsync(User, matchGroupUser.MatchGroupId, attributes);

            return await BaseDeleteAsync(id);
        }

        [Route("get/id/{id:int}")]
        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            return await BaseGetByIdAsync(id);
        }

        [Route("getallusers/matchgroup/{matchGroupId:int}")]
        [HttpGet]
        public Task<IActionResult> GetAllUsersByMatchGroupId(int matchGroupId)
        {
            return CommonOperationAsync<IActionResult>(async () =>
            {
                var result = await Manager.GetAllUsersByMatchGroupIdAsync(matchGroupId);

                return Ok(new ApiResponse(LocalizationService, Logger).Ok(
                    Mapper.Map<IList<MatchGroupUser>, IList<MatchGroupUserResponse>>(result)));
            });
        }

        [Route("getalluser/matchgroup/{matchGroupId:int}")]
        [HttpGet]
        public Task<IActionResult> GetUsersByMatchGroupId(int matchGroupId)
        {
            return CommonOperationAsync<IActionResult>(async () =>
            {
                var result = await Manager.GetUsersByMatchGroupIdAsync(matchGroupId);

                return Ok(new ApiResponse(LocalizationService, Logger).Ok(
                    Mapper.Map<IList<User>, IList<UserResponse>>(result)));
            });
        }

        [Route("getallmatchgroup/user/{userId:int}")]
        [HttpGet]
        public Task<IActionResult> GetMatchGroupsByUserId(int userId)
        {
            return CommonOperationAsync<IActionResult>(async () =>
            {
                var result = await Manager.GetMatchGroupsByUserIdAsync(userId);

                return Ok(new ApiResponse(LocalizationService, Logger).Ok(
                    Mapper.Map<IList<MatchGroup>, IList<MatchGroupResponse>>(result)));
            });
        }
    }
}