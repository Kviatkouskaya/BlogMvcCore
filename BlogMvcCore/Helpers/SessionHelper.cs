using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace BlogMvcCore.Helpers
{
    public static class SessionHelper
    {
        public static void SetUserAsJson(this ISession session, string key, object value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value));
        }
        public static User GetUserFromJson<User>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(User) : JsonConvert.DeserializeObject<User>(value);
        }
    }
}
