using Microsoft.AspNetCore.Identity;
using RemindMeApp.Server.Data;
using RemindMeApp.Shared;

namespace RemindMeApp.Server.Authentication;

public interface IUserService
{
    Task<AuthToken?> CreateUserAsync(UserInfo newUser);

    Task<AuthToken?> GetTokenAsync(UserInfo userInfo);

    Task<AuthToken?> GetOrCreateUserAsync(string provider, ExternalUserInfo userInfo);
}

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;

    public UserService(UserManager<ApplicationUser> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<AuthToken?> CreateUserAsync(UserInfo newUser)
    {
        var identityResult = await _userManager.CreateAsync(new ApplicationUser { UserName = newUser.Username }, newUser.Password);

        if (!identityResult.Succeeded)
        {
            return null;
        }

        return await GetTokenAsync(newUser);
    }

    public async Task<AuthToken?> GetTokenAsync(UserInfo userInfo)
    {
        var user = await _userManager.FindByNameAsync(userInfo.Username);

        if (user is null || !await _userManager.CheckPasswordAsync(user, userInfo.Password))
        {
            return null;
        }

        return _tokenService.GenerateToken(user.UserName!);
    }
    public async Task<AuthToken?> GetOrCreateUserAsync(string provider, ExternalUserInfo userInfo)
    {
        var user = await _userManager.FindByLoginAsync(provider, userInfo.ProviderKey);

        var result = IdentityResult.Success;

        if (user is null)
        {
            user = new ApplicationUser { UserName = userInfo.Username };

            result = await _userManager.CreateAsync(user);

            if (result.Succeeded)
            {
                result = await _userManager.AddLoginAsync(user, new UserLoginInfo(provider, userInfo.ProviderKey, displayName: null));
            }
        }

        if (result.Succeeded)
        {
            return _tokenService.GenerateToken(user.UserName!);
        }

        return null;
    }
}
