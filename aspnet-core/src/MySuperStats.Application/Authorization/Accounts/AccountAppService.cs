using System.Linq;
using System.Threading.Tasks;
using Abp.Configuration;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.UI;
using Abp.Zero.Configuration;
using Microsoft.EntityFrameworkCore;
using MySuperStats.Authorization.Accounts.Dto;
using MySuperStats.Authorization.Users;

namespace MySuperStats.Authorization.Accounts
{
    public class AccountAppService : MySuperStatsAppServiceBase, IAccountAppService
    {
        // from: http://regexlib.com/REDetails.aspx?regexp_id=1923
        public const string PasswordRegex =
            "(?=^.{8,}$)(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?!.*\\s)[0-9a-zA-Z!@#$%^&*()]*$";

        private readonly UserRegistrationManager _userRegistrationManager;
        private readonly IRepository<User, long> _userRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public AccountAppService(
            UserRegistrationManager userRegistrationManager, IRepository<User, long> userRepository,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _userRegistrationManager = userRegistrationManager;
            _userRepository = userRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public async Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input)
        {
            var tenant = await TenantManager.FindByTenancyNameAsync(input.TenancyName);
            if (tenant == null)
            {
                return new IsTenantAvailableOutput(TenantAvailabilityState.NotFound);
            }

            if (!tenant.IsActive)
            {
                return new IsTenantAvailableOutput(TenantAvailabilityState.InActive);
            }

            return new IsTenantAvailableOutput(TenantAvailabilityState.Available, tenant.Id);
        }

        public async Task<RegisterOutput> Register(RegisterInput input)
        {
            using (_unitOfWorkManager.Current.DisableFilter(AbpDataFilters.MayHaveTenant,
                AbpDataFilters.MustHaveTenant))
            {
                //Check username or email

                if (await _userRepository.GetAll()
                    .Where(p => p.UserName == input.UserName || p.EmailAddress == input.EmailAddress).AnyAsync())
                {
                    throw new UserFriendlyException(L("UserNameOrEmailInUse"));
                }
            }

            using (_unitOfWorkManager.Current.SetTenantId(1))
            {
                AbpSession.Use(1, null);
            
                var user = await _userRegistrationManager.RegisterAsync(
                    input.Name,
                    input.Surname,
                    input.EmailAddress,
                    input.UserName,
                    input.Password,
                    true // Assumed email address is always confirmed. Change this if you want to implement email confirmation.
                );

                var isEmailConfirmationRequiredForLogin =
                    await SettingManager.GetSettingValueAsync<bool>(AbpZeroSettingNames.UserManagement
                        .IsEmailConfirmationRequiredForLogin);

                return new RegisterOutput
                {
                    CanLogin = user.IsActive && (user.IsEmailConfirmed || !isEmailConfirmationRequiredForLogin)
                };                
            }
        }
    }
}