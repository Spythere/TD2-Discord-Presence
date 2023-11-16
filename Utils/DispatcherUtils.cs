namespace TD2_Presence.Utils
{

    enum DispatcherStatus
    {
        INVALID = -2,
        UNKNOWN = -1,
        NO_LIMIT_OR_DEFINED = 0,
        AFK = 1,
        ENDING = 2,
        NO_SPACE = 3,
        UNAVAILABLE = 4,
        NOT_LOGGED_IN = 5,
    }

    public static class DispatcherUtils
    {
        public static string getDispatcherStatus(long statusCode)
        {
            if (statusCode >= -2 && statusCode <= 5)
                return ResourceUtils.GetRPC($"Status Code {statusCode}");

            if (statusCode >= new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() + 25500000)
                return ResourceUtils.GetRPC("Status Code 0");

            return $"{ResourceUtils.GetRPC("Status Code Available")} {DateTimeOffset.FromUnixTimeMilliseconds(statusCode).DateTime.ToLocalTime():H:mm}";
        }
    }
}
