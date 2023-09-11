using System.Globalization;

namespace TD2_Presence.Utils
{

    public static class DispatcherUtils
    {
        public static string getDispatcherStatus(long statusCode)
        {
            if (statusCode >= -2 && statusCode <= 4)
                return ResourceUtils.Get($"Status Code {statusCode}");

            return $"{ResourceUtils.Get("Status Code Available")} {DateTimeOffset.FromUnixTimeMilliseconds(statusCode).DateTime.ToLocalTime().ToString("H:mm")}";
        }
    }
}
