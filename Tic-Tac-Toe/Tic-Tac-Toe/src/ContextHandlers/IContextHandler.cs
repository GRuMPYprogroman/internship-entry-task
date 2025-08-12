namespace Tic_Tac_Toe.ContextHandlers;

public interface IContextHandler
{
    public Task<IResult> Handle(HttpContext context);
}