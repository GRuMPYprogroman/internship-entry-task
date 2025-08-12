using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Tic_Tac_Toe.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Tic_Tac_Toe.ContextHandlers;
using Tic_Tac_Toe.DbComponents;

class Program
{
    static async Task Main(string[] args)
    {
        try
        {
            var builder = WebApplication.CreateBuilder(args);
            
            string? connectionString = builder.Configuration.GetConnectionString("Default");
            if (connectionString == null) throw new InvalidDataException("Missing connection string in configuration.");
            
            builder.Services.AddDbContext<ApplicationContext>(options => options.UseNpgsql(connectionString));
            builder.Services.AddCors();
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/login";
                    options.LogoutPath = "/logout";
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SameSite = SameSiteMode.Lax;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                    options.ExpireTimeSpan = TimeSpan.FromDays(7);
                });
            builder.Services.AddAuthorization();
            
            HandlersRegistrationManager.RegisterHandlers(builder);
            
            var app = builder.Build();
            
            app.UseCors(builder => builder.AllowAnyOrigin());
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStaticFiles();
            
            
            app.MapGet("/", (HttpContext context) =>
            {
                if (context.User.Identity?.IsAuthenticated == true)
                    return Results.Redirect("/games");
                
                Console.WriteLine("Not authenticated");
                
                return Results.Redirect("/login");
            });
            app.MapGet("/logout", async (HttpContext context, LogoutHandler handler) => await handler.Handle(context));
            app.MapGet("/login", (HttpContext context) => 
            {
                if (context.User.Identity?.IsAuthenticated == true)
                    return Results.Redirect("/games");

                return Results.File(
                    "login.html",
                    "text/html"
                );
            });
            app.MapGet("/games", [Authorize] async (HttpContext context, ISessionsDb sessions) =>
            {
                // Если запрос с Accept: text/html
                if (context.Request.Headers.Accept.Any(h => h.Contains("text/html")))
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "Private", "games.html");
                    if (!File.Exists(path))
                        return Results.NotFound();

                    var bytes = await File.ReadAllBytesAsync(path);
                    return Results.File(bytes, "text/html");
                }
                 // Если это AJAX запрос с fetch - вернём список игр
                var gameList = sessions.Sessions.Select(s => new
                {
                    gameId = s.GameId,
                    hostId = s.HostId,
                    isEmpty = s.isEmpty
                }).ToList();

                return Results.Json(gameList);
            });
            app.MapGet("/profile",   [Authorize](HttpContext context) => Results.Json(new { userId = context.User.Identity?.Name }));
            app.MapGet("/game/{id}", [Authorize] async (string id, HttpContext context, ISessionsDb sessions) => 
            {
                // Если запрос с Accept: text/html
                if (context.Request.Headers.Accept.Any(h => h.Contains("text/html")))
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "Private", "game.html");
                    if (!File.Exists(path))
                        return Results.NotFound();

                    var bytes = await File.ReadAllBytesAsync(path);
                    return Results.File(bytes, "text/html");
                }
                // Если это AJAX запрос с fetch - вернём информацию об игре
                var game = sessions.Sessions.FirstOrDefault(s => s.GameId == id);
                if (game == null) return  Results.NotFound();

                var board = game._board.GetField();
                var status = game.Status;
                var turn = game.CurrentTurn; 

                return Results.Json(new {board = board, status = status, currentTurn = turn});
            });

            app.MapPost("/register", async (HttpContext context, RegistrationHandler handler) => await handler.Handle(context));
            app.MapPost("/login", async (HttpContext context, LoginHandler handler) => await handler.Handle(context));
            app.MapPost("/games", [Authorize] async (HttpContext context, GameCreationHandler handler) =>  await handler.Handle(context));
            app.MapPost("/games/{id}/join", [Authorize](string id, HttpContext context, ISessionsDb sessions) =>
            {
                var userId = context.User.Identity?.Name;
                var game = sessions.Sessions.FirstOrDefault(s => s.GameId == id);
                if (game == null) return  Results.NotFound();
                if (!game.isEmpty) return Results.Conflict("Game is full");

                game.SessionIdPlayer2 = userId;
                
                return Results.Ok();
            });
            app.MapPost("/games/{id}/moves", [Authorize] async (HttpContext context, MoveHandler handler) => await handler.Handle(context));
        
            app.Run();
        }
        catch (InvalidDataException ex)
        {
            Console.WriteLine("Server faced problem: " + ex.Message);
        }
    }
}

