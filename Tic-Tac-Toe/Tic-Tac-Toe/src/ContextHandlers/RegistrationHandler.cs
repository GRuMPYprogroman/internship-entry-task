using Microsoft.EntityFrameworkCore;
using Tic_Tac_Toe.Components;
using Tic_Tac_Toe.DbComponents;

namespace Tic_Tac_Toe.ContextHandlers;

public class RegistrationHandler : IContextHandler
{
    private readonly ApplicationContext _db;
    
    public RegistrationHandler(ApplicationContext db)
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
        
        User? existingUser  = await _db.Users.FirstOrDefaultAsync(x => x.Username == username);
        if (existingUser  != null)
            return Results.Conflict("Username already exists");
        
        var newUser = new User {
            Id = Guid.NewGuid(),
            Username = username,
            PasswordHash = password.HashSha256Password(),
            CreatedAt = DateTime.UtcNow
        };
        
        await _db.Users.AddAsync(newUser);
        await _db.SaveChangesAsync();
        
        return Results.Ok(new {
            sessionId = newUser.Id,
            username = newUser.Username
        });
    }

}