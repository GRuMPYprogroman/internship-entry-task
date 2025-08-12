using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Tic_Tac_Toe.ContextHandlers;

public class LogoutHandler : IContextHandler
{
    public async Task<IResult> Handle(HttpContext context)
    {
        await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Results.Redirect("/login");
    }
}