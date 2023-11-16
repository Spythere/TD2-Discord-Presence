using TD2_Presence.Utils;

namespace TD2_Presence.Managers
{
    public static class LanguageManager
    {
        public static void RunLanguageChoice(bool newChoice = false)
        {
            string? language = ConfigManager.ReadValue("language");

            if (language != null && !newChoice)
            {
                ResourceUtils.SetCulture(language);
                return;
            }

            Menu.ExitModeEnum exitMode = newChoice ? Menu.ExitModeEnum.NONE : Menu.ExitModeEnum.NONE;

            string[] options = { "POLSKI", "ENGLISH", "ČEŠTINA (překlad: matseb)", "DEUTSCH (Übersetzung: Bravura Lion)" };
            Menu menu = new Menu("WYBÓR JĘZYKA / LANGUAGE CHOICE / VÝBĚR JAZYKA / SPRACHAUSWAHL", options, exitMode);

            int SelectedIndex = menu.Run();
            string appCultureName = GetCultureName(SelectedIndex);

            ResourceUtils.SetCulture(appCultureName);
            ConfigManager.SetValue("language", appCultureName);
 
            Console.Clear(); 
        }

        public static void RunRPCLanguageChoice(bool newChoice = false)
        {
            string? rpcLanguage = ConfigManager.ReadValue("rpcLanguage");

            if (rpcLanguage != null && !newChoice)
            {
                ResourceUtils.SetRPCCulture(rpcLanguage);
                return;
            }

            Menu.ExitModeEnum exitMode = newChoice ? Menu.ExitModeEnum.NONE : Menu.ExitModeEnum.NONE;

            string[] options = { "POLSKI", "ENGLISH", "ČEŠTINA (překlad: matseb)", "DEUTSCH (Übersetzung: Bravura Lion)" };
            Menu menu = new Menu(ResourceUtils.Get("Presence Language Change Info"), options, exitMode);

            int SelectedIndex = menu.Run();
            string rpcCultureName = GetCultureName(SelectedIndex);

            ResourceUtils.SetRPCCulture(rpcCultureName);
            ConfigManager.SetValue("rpcLanguage", rpcCultureName);

            Console.Clear();
        }

        static string GetCultureName(int selectedIndex, string? defaultCulture = null)
        {
            return selectedIndex switch
            {
                1 => "en-US",
                2 => "cs-CZ",
                3 => "de-DE",
                _ => defaultCulture ?? "pl-PL",
            };
        }
    }
}
