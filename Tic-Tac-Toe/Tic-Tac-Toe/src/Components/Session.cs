namespace Tic_Tac_Toe.Components;

public class Session
{
    public string GameId { get; } = Guid.NewGuid().ToString();
    public string HostId { get; }
    public string? SessionIdPlayer2 { get; set; }
    
    public bool isEmpty => SessionIdPlayer2 == null;
    
    public readonly Board _board;
    public string Status { get; set; }
    public string? CurrentTurn;
    
    public Session(string host, int size)
    {
        _board =  new Board(size);
        HostId = host;
        CurrentTurn = host;
        Status = "X's turn";
    }

    public void SwitchTurn()
    {
        if (SessionIdPlayer2 == null)
        {
            Status = "Waiting for opponent";
            return;
        }
        CurrentTurn = (CurrentTurn == SessionIdPlayer2) ? HostId : SessionIdPlayer2;
    }
}