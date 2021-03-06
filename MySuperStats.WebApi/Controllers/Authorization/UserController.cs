using AutoMapper;
using MySuperStats.WebApi.ApplicationSettings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MySuperStats.WebApi.Models;
using System.Collections.Generic;
using MySuperStats.Contracts.Responses;
using System.Threading.Tasks;
using MySuperStats.WebApi.Enums;
using System;
using MySuperStats.Contracts.Requests;
using MySuperStats.WebApi.Business;
using MySuperStats.Contracts.Enums;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using CustomFramework.BaseWebApi.Utils.Controllers;
using CustomFramework.BaseWebApi.Resources;
using CustomFramework.BaseWebApi.Authorization.Attributes;
using CustomFramework.BaseWebApi.Authorization.Enums;
using CustomFramework.BaseWebApi.Utils.Contracts;
using CustomFramework.EmailProvider;
using CustomFramework.BaseWebApi.Utils.Utils.Exceptions;

namespace MySuperStats.WebApi.Controllers.Authorization
{
    [ApiExplorerSettings(IgnoreApi = false)]
    [Route(ApiConstants.DefaultRoute + nameof(User))]
    public class UserController : BaseController
    {
        private readonly IUserManager _userManager;
        private readonly IPlayerManager _playerManager;
        private readonly IPermissionChecker _permissionChecker;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserController(IHttpContextAccessor httpContextAccessor, IPermissionChecker permissionChecker, ILocalizationService localizationService, ILogger<Controller> logger, IMapper mapper, IUserManager userManager, IEmailSender emailSender, IPlayerManager playerManager)
            : base(localizationService, logger, mapper)
        {
            _userManager = userManager;
            _permissionChecker = permissionChecker;
            _httpContextAccessor = httpContextAccessor;
            _playerManager = playerManager;
        }

        [Route("{id:int}/update/email/request")]
        [HttpPut]
        public async Task<IActionResult> UpdateEmailAsync(int id, [FromBody] UserEmailUpdateRequest request)
        {
            var loggedUserId = Convert.ToInt32(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (loggedUserId != id) //Eğer giriş yapan kullanıcı, farklı kullanıcıya ait bilgileri güncellemek istiyorsa yetkisi kontrol ediliyor
            {
                var attributes = new List<PermissionAttribute> {
                    new PermissionAttribute(nameof(PermissionEnum.UpdateEmail), nameof(BooleanEnum.True))
                 };
                await _permissionChecker.HasPermissionAsync(User, 0, attributes);
            }

            if (!ModelState.IsValid)
                throw new ArgumentException(ModelState.ModelStateToString(LocalizationService));

            var result = await CommonOperationAsync<bool>(async () =>
            {
                var user = await _userManager.GetByIdAsync(id);
                if (user == null)
                    throw new KeyNotFoundException(nameof(User));

                await _userManager.GenerateTokenForChangeEmailAsync(user, request.NewEmail, Url, Request.Scheme);
                return true;
            });

            return Ok(new ApiResponse(LocalizationService, Logger).Ok(result));
        }

        [Route("update/email/confirm/userId/{userId:int}/newEmail/{email}/code/{code}")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateEmailConfirmationAsync(int userId, string email, string code)
        {
            var result = await _userManager.ChangeEmailAsync(userId, email, code);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                throw new ArgumentException($"{LocalizationService.GetValue("Email confirmation error")}: {ModelState.ModelStateToString(LocalizationService)}"); //Error confirming email for user with ID '{userId}':
            }

            return Ok("Yeni E-Posta adresiniz onaylanmıştır");
        }

        [Route("delete/{id:int}")]
        [HttpDelete]
        [Permission(nameof(SpecialEnums.OnlyAdmin), nameof(BooleanEnum.True))]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            var player = await _userManager.GetPlayerByIdAsync(id);

            await _userManager.DeleteAsync(id);
            await _playerManager.DeleteAsync(player.Id);

            return Ok(new ApiResponse(LocalizationService, Logger).Ok(true));
        }

        [Route("get/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var result = await CommonOperationAsync<User>(async () =>
            {
                return await _userManager.GetByIdAsync(id);
            });
            return Ok(new ApiResponse(LocalizationService, Logger).Ok(Mapper.Map<User, UserResponse>(result)));
        }

        [Route("getplayer/{id}")]
        [HttpGet]
        public async Task<IActionResult> GetPlayerByIdAsync(int id)
        {
            var result = await CommonOperationAsync<Player>(async () =>
            {
                return await _userManager.GetPlayerByIdAsync(id);
            });
            return Ok(new ApiResponse(LocalizationService, Logger).Ok(Mapper.Map<Player, PlayerResponse>(result)));
        }

        [Route("get/email/{emailAddress}")]
        [HttpGet]
        public async Task<IActionResult> GetByEmailAddressAsync(string emailAddress)
        {
            var result = await CommonOperationAsync<User>(async () =>
            {
                return await _userManager.GetByEmailAddressAsync(emailAddress);
            });
            return Ok(new ApiResponse(LocalizationService, Logger).Ok(Mapper.Map<User, UserResponse>(result)));
        }

        [Route("getall/matchgroupid/{matchGroupId:int}")]
        [HttpGet]
        public async Task<IActionResult> GetAllByMatchGroupIdAsync(int matchGroupId)
        {
            var result = await CommonOperationAsync<IList<User>>(async () =>
            {
                return await _userManager.GetAllByMatchGroupIdAsync(matchGroupId);
            });

            return Ok(new ApiResponse(LocalizationService, Logger).Ok(
                Mapper.Map<IList<User>, IList<UserResponse>>(result), result.Count));
        }
    }
}
