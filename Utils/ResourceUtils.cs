using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Resources;
using TD2_Presence.Classes;

namespace TD2_Presence.Utils
{
    public static class ResourceUtils
    {

        private static readonly ResourceManager rm = new ResourceManager("TD2_Presence.Resources.Resources", Assembly.GetExecutingAssembly());
        private static CultureInfo currentCultureInfo = new CultureInfo("pl-PL");
        private static CultureInfo currentRpcCultureInfo = new CultureInfo("pl-PL");

        public static void SetCulture(string cultureName)
        {
            currentCultureInfo = new CultureInfo(cultureName);
        }

        public static void SetRPCCulture(string rpcCultureName)
        {
            currentRpcCultureInfo = new CultureInfo(rpcCultureName);
        }

        public static string Get(string name)
        {
            return rm.GetString(name, currentCultureInfo)!;
        }

        public static string GetRPC(string name)
        {
            return rm.GetString(name, currentRpcCultureInfo)!;
        }
    }
}
