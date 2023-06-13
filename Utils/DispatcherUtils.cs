using System.Globalization;

namespace TD2_Presence.Utils
{

    public static class DispatcherUtils
    {
        static Dictionary<long, string> statuses = new Dictionary<long, string>()
        {
            {-2, "NIEWPISANY"},
            { -1, "NIEDOSTĘPNY"},
            { 0, "BEZ LIMITU"},
            { 1, "Z/W"},
            { 2, "KOŃCZY"},
            { 3, "BRAK MIEJSCA"},
            { 4, "NIEZNANY"},
        };

        public static string getDispatcherStatus(long statusCode)
        {
            if (statusCode >= -2 && statusCode <= 4)
                return statuses[statusCode];

            return $"DOSTĘPNY DO {DateTimeOffset.FromUnixTimeMilliseconds(statusCode).DateTime.ToLocalTime().ToString("H:mm")}";
        }
    }
}
