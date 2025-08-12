using Tic_Tac_Toe.Components;

namespace Tic_Tac_Toe.DbComponents;

public class SessionsList : ISessionsDb
{
    private List<Session> sessions = new List<Session>();
    public IEnumerable<Session> Sessions { get => sessions; }

    public void AddSession(Session session)
    {
        sessions.Add(session);
    }
}