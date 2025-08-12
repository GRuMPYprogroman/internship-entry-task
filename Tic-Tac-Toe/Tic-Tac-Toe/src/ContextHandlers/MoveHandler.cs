using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Expressions.Internal;
using Tic_Tac_Toe.DbComponents;

namespace Tic_Tac_Toe.ContextHandlers;

public class MoveHandler : IContextHandler
{
    private readonly ISessionsDb _sessionsDb;
    
    public MoveHandler(ISessionsDb sessionsDb)
    {
        _sessionsDb = sessionsDb;
    }
    
    public async Task<IResult> Handle(HttpContext context)
    {
        var move = await context.Request.ReadFromJsonAsync<MoveDto>();
        if (move == null) return Results.BadRequest();
        
        var id = context.Request.RouteValues["id"]?.ToString();
        if (string.IsNullOrEmpty(id)) return Results.BadRequest("Game id is missing");
        Console.Write(id);
        
        var userId = context.User.Identity!.Name;
        
        var game = _sessionsDb.Sessions.FirstOrDefault(s => s.GameId == id);
        if (game == null) return Results.NotFound();
        if (game.SessionIdPlayer2 == null)
        {
            game.Status = "Waiting for second player";
            return Results.Ok(new { status = game.Status, board = game._board.GetField(), currentTurn = game.CurrentTurn });
        }
        
        var symbol = (userId == game.HostId) ? "X" : "O";
        
        if (Random.Shared.Next(0, 100) < 10)
        {
            symbol = symbol == "X" ? "O" : "X";
        }
        
        if (game.CurrentTurn != userId) return Results.Conflict("Not your turn");
        
        // Преобразуем index в координаты
        int size = game._board._size;
        int row = move.Index / size;
        int col = move.Index % size;

        // Проверка, что клетка свободна
        if (game._board[row, col] != null)
            return Results.Conflict("Cell is already occupied");
        
        game._board[row, col] = symbol;

        game._board.CheckWinner(out string? winner);
        if (winner != null)
        {
            game.Status = $"{symbol} wins!";
            return Results.Ok(new { status = game.Status, winner });
        }

        if (game._board.CheckDraw())
        {
            game.Status = "Draw";
            return Results.Ok(new { status = game.Status, draw = true });
        }

        game.SwitchTurn();
        
        userId = context.User.Identity!.Name;
        
        if (userId == game.CurrentTurn)  game.Status =  $"{userId}'s turn";
        
        return Results.Ok(new { status = game.Status, board = game._board.GetField(), currentTurn = game.CurrentTurn });
    }

    private record MoveDto(int Index);
}