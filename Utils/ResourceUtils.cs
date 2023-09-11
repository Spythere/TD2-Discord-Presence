using System.Resources;

namespace TD2_Presence.Utils
{
    public static class ResourceUtils
    {

        static ResourceManager rm = new ResourceManager("TD2_Presence.Resources.Resources", typeof(Program).Assembly);

        public static string Get(string name)
        {
            return rm.GetString(name)!;
        }
    }
}
