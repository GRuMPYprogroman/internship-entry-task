using Tic_Tac_Toe.Components;
using Tic_Tac_Toe.DbComponents;

namespace Tic_Tac_Toe.ContextHandlers;

public class GameCreationHandler : IContextHandler
{
    private readonly ISessionsDb _sessionsDb;
    
    public GameCreationHandler(ISessionsDb sessionsDb)
    {
        _sessionsDb = sessionsDb;
    }
    
    public async Task<IResult> Handle(HttpContext context)
    {
        var userId = context.User.Identity!.Name;

        var body = await context.Request.ReadFromJsonAsync<CreateGameRequest>();
        if (body == null || body.Size < 3 || body.Size > 10 )
            return Results.BadRequest(new { error = "Size must be between 3 and 10" });

        var newGame = new Session(userId, body.Size);

        _sessionsDb.AddSession(newGame);
        
        return Results.Json(new {gameId = newGame.GameId});
    }
    
    private record CreateGameRequest(int Size);
}