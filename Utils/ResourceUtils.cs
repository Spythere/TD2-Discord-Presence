using System.Globalization;
using System.Reflection;
using System.Resources;

namespace TD2_Presence.Utils
{
    public static class ResourceUtils
    {

        private static readonly ResourceManager rm = new ResourceManager("TD2_Presence.Resources.Resources", Assembly.GetExecutingAssembly());
        private static CultureInfo currentCultureInfo = new CultureInfo("pl-PL");

        public static void SetCulture(string cultureName)
        {
            currentCultureInfo = new CultureInfo(cultureName);
        }

        public static string Get(string name)
        {
            return rm.GetString(name, currentCultureInfo)!;
        }
    }
}
