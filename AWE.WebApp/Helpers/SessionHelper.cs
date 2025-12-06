#nullable disable
using AWE.Models;
using System.Text.Json;

namespace AWE.WebApp.Helpers
{
    public static class SessionHelper
    {
        private const string UserSessionKey = "CurrentUser";

        public static void SetCurrentUser(ISession session, User user)
        {
            session.SetString(UserSessionKey, JsonSerializer.Serialize(user));
        }

        public static User? GetCurrentUser(ISession session)
        {
            var userJson = session.GetString(UserSessionKey);
            return userJson == null ? null : JsonSerializer.Deserialize<User>(userJson);
        }

        public static bool IsAuthenticated(ISession session)
        {
            return session.GetString(UserSessionKey) != null;
        }

        public static void ClearSession(ISession session)
        {
            session.Clear();
        }
    }
}
