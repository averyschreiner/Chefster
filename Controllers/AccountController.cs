using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

public class AccountController : Controller
{
    public async Task LogIn()
    {
        var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
            .WithRedirectUri(Url.Action("AfterLogIn", "Account"))
            .Build();

        await HttpContext.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
    }

    [Authorize]
    public async Task LogOut()
    {
        var authenticationProperties = new LogoutAuthenticationPropertiesBuilder()
            .WithRedirectUri(Url.Action("Index", "Home"))
            .Build();

        await HttpContext.SignOutAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }

    public async Task SignUp()
    {
        var authenticationProperties = new LoginAuthenticationPropertiesBuilder()
            .WithParameter("screen_hint", "signup")
            .WithRedirectUri(Url.Action("AfterSignUp", "Account"))
            .Build();

        await HttpContext.ChallengeAsync(Auth0Constants.AuthenticationScheme, authenticationProperties);
    }

    [Authorize]
    public IActionResult AfterSignUp()
    {
        return View();
    }

    [Authorize]
    public IActionResult AfterLogIn()
    {
        return View();
    }
}