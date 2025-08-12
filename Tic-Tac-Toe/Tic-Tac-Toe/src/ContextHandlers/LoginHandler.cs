using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Tic_Tac_Toe.Components;
using Tic_Tac_Toe.DbComponents;

namespace Tic_Tac_Toe.ContextHandlers;

public class LoginHandler : IContextHandler
{
    private readonly ApplicationContext _db;
    
    public LoginHandler(ApplicationContext db)
    {
        _db = db;
    }

    public async Task<IResult> Handle(HttpContext context)
    {
        var userData = await context.Request.ReadFromJsonAsync<RegisterRequest>();
        
        if (userData is null || string.IsNullOrWhiteSpace(userData.Username) || string.IsNullOrWhiteSpace(userData.Password))
            return Results.BadRequest("Invalid username or password");
        
        string username = userData.Username;
        string password = userData.Password;
        
        Console.WriteLine(username + " " + password.HashSha256Password());
        
        User? existingUser  = await _db.Users.FirstOrDefaultAsync(x => x.Username == username && x.PasswordHash == password.HashSha256Password());
        if (existingUser is null)
        {
            Console.WriteLine("NO USER FOUND IN DB");
            return Results.Unauthorized();
        }
        
        var claims = new List<Claim> {new Claim(ClaimTypes.Name, username)};
        ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
        
        string? returnUrl = context.Request.Query["returnUrl"];
        return Results.Ok(new { redirectTo = returnUrl ?? "/games" });
    }
}