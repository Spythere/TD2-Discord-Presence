using System.Reflection;
using TD2_Presence;
using TD2_Presence.Managers;
using TD2_Presence.Utils;

PresenceProgram presenceProgram = new PresenceProgram();
presenceProgram.SetupCulture();
presenceProgram.SetupProgram();

class PresenceProgram
{
    Version? currentVersion;
    bool mainLoop = true;

    public PresenceProgram() { 
        currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
    }

    public void SetupCulture()
    {
        ResourceUtils.SetCulture("pl-PL");
    }

    public void SetupProgram()
    {
        /* Presence handlers */
        AppDomain.CurrentDomain.ProcessExit += new EventHandler(PresenceTimer.OnProcessExit!);

        /* Console app title */
        Console.Title = $"{ResourceUtils.Get("Title")} ({currentVersion})";

        /* Config file setup */
        ConfigManager.SetupConfig();

        /* Language choice */
        LanguageChoice();

        /* Update checking */
        Console.WriteLine(ResourceUtils.Get("Update Checking"));
        Task.Run(() => UpdaterUtils.CheckForUpdates()).Wait();
        Console.Clear();

        /* Initial messages */
        Console.WriteLine($"==== TD2 Discord Presence (v{currentVersion}) by Spythere ====");
        ConsoleUtils.WriteWarning(ResourceUtils.Get("Initial Info 1")!);
        ConsoleUtils.WriteWarning(ResourceUtils.Get("Initial Info 2")!);
        Console.WriteLine("==========================================");
        Console.WriteLine();
        
        /* Program main loop */
        runMainLoop();
    }

    private void LanguageChoice(bool newChoice = false)
    {
        string? language = ConfigManager.ReadValue("language");
        if (language != null && !newChoice)
        {
            ResourceUtils.SetCulture(language);
            return;
        }

        ConsoleUtils.WriteInfo("[PL] Wybierz język. Będzie on dotyczyć tylko tekstu w konsoli. Informacje Discord Presence będą pokazane w języku polskim.");
        ConsoleUtils.WriteInfo("[EN] Select a language. It will apply to the console text only. Discord Presence info will be shown in Polish.");
        ConsoleUtils.WriteInfo("[DE] Wählen Sie eine Sprache. Sie gilt nur für den Text in der Konsole. Die Discord-Anwesenheitsinfo wird auf Polnisch angezeigt.");
        ConsoleUtils.WriteInfo("[CZ] Vyber jazyk. Překlad se bude týkat pouze textu v konzoli. Discord Presence bude informace ukazovat pouze v polštině.");
        Console.WriteLine();
        ConsoleUtils.WritePrompt("Język / Language (1 - POLSKI; 2 - ENGLISH; 3 - DEUTSCH; 4 - ČEŠTINA): ");
        
        char key = Console.ReadKey().KeyChar;

        string languageToSet = "pl-PL";

        switch(key)
        {
            case '2':
                languageToSet = "en-US";
                break;
            case '3':
                languageToSet = "de-DE";
                break;
            case '4':
                languageToSet = "cs-CZ";
                break;
        }

        ResourceUtils.SetCulture(languageToSet);
        ConfigManager.SetValue("language", languageToSet);

        if(!newChoice)
        {
            Console.Clear();
        }
    }

    private void runMainLoop()
    {
        while (mainLoop)
        {
            ConsoleUtils.WritePrompt(ResourceUtils.Get("Mode Choice Info")!);
            char key = Console.ReadKey().KeyChar;
            Console.WriteLine();

            switch (key)
            {
                case '1':
                case '2':

                    string? savedUsername = ConfigManager.ReadValue("savedUsername");

                    if (savedUsername != null)
                        ConsoleUtils.WritePrompt(string.Format(ResourceUtils.Get("Username Prompt With Default")!, savedUsername));
                    else
                        ConsoleUtils.WritePrompt(ResourceUtils.Get("Username Prompt")!);

                    string? username = Console.ReadLine();

                    if (string.IsNullOrEmpty(savedUsername))
                    {
                        while (string.IsNullOrWhiteSpace(username))
                        {
                            ConsoleUtils.WriteWarning(ResourceUtils.Get("Incorrect Username Warning")!);
                            ConsoleUtils.WritePrompt(ResourceUtils.Get("Username Prompt")!);
                            username = Console.ReadLine();
                        }
                    }
                    else
                        username = string.IsNullOrWhiteSpace(username) ? savedUsername : username;

                    ConfigManager.SetValue("savedUsername", username);

                    PresenceManager.InitializePresence();
                    PresenceTimer.Run(key == '1' ? PresenceMode.DISPATCHER : PresenceMode.DRIVER, username);

                    break;

                case '3':
                    ConsoleUtils.WritePrompt(ResourceUtils.Get("Scenery Name Prompt")!);
                    string? sceneryName = Console.ReadLine();

                    while (string.IsNullOrWhiteSpace(sceneryName))
                    {
                        ConsoleUtils.WritePrompt(ResourceUtils.Get("Scenery Name Prompt")!);
                        sceneryName = Console.ReadLine();
                    }

                    PresenceManager.InitializePresence();
                    PresenceManager.ShowPresenceEditorData(sceneryName);

                    break;

                case '4':
                    LanguageChoice(true);
                    Console.Clear();
                    continue;

                default:
                    System.Environment.Exit(0);
                    break;
            }

            ConsoleUtils.WriteWarning(ResourceUtils.Get("Change Settings Info")!);
            ConsoleKey confirmKey = Console.ReadKey().Key;

            PresenceTimer.Stop();
            PresenceManager.ResetPresenceData();

            if (confirmKey != ConsoleKey.Enter)
                mainLoop = false;
        }
    }
}





