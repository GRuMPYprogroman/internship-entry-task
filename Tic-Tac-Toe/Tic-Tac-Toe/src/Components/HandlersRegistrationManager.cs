using Tic_Tac_Toe.ContextHandlers;
using Tic_Tac_Toe.DbComponents;

namespace Tic_Tac_Toe.Components;

public static class HandlersRegistrationManager
{
    public static void RegisterHandlers(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<LoginHandler>();
        builder.Services.AddScoped<RegistrationHandler>();
        builder.Services.AddScoped<LogoutHandler>();
        builder.Services.AddSingleton<ISessionsDb, SessionsList>();
        builder.Services.AddScoped<GameCreationHandler>();
        builder.Services.AddScoped<MoveHandler>();
    }
}