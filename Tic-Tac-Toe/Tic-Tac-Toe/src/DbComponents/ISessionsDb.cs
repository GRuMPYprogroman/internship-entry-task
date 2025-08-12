using Tic_Tac_Toe.Components;

namespace Tic_Tac_Toe.DbComponents;

public interface ISessionsDb
{
    IEnumerable<Session> Sessions { get; }
    void AddSession(Session session);
}