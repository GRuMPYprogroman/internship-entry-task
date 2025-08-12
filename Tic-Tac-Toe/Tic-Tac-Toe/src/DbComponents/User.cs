namespace Tic_Tac_Toe.DbComponents;

public class User
{
    public Guid Id { get; set; }
    public required string Username { get; set; }
    public required string PasswordHash { get; set; }
    public int GamesWon { get; set; }
    public int GamesLost { get; set; }
    public DateTime CreatedAt { get; set; }
}