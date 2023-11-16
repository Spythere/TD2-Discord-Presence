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

    public PresenceProgram()
    {
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
        LanguageManager.RunLanguageChoice();
        LanguageManager.RunRPCLanguageChoice();

        /* Update checking */
        Console.WriteLine(ResourceUtils.Get("Update Checking"));
        Task.Run(() => UpdaterUtils.CheckForUpdates()).Wait();
        Console.Clear();

        /* Initial messages */
        RunInitMessage();
        

        /* Program main loop */
        // runMainLoop();

        RunMainMenu();

        PresenceTimer.Stop();
        PresenceManager.ResetPresenceData();

        System.Environment.Exit(0);

    }

    private void RunInitMessage()
    {
        string? initInfoShown = ConfigManager.ReadValue("initInfo");

        if (initInfoShown != null)
            return;
        
        RenderLogo();

        ConsoleUtils.WriteWarning(ResourceUtils.Get("Initial Info 1")!);
        Console.WriteLine();
        ConsoleUtils.WriteWarning(ResourceUtils.Get("Initial Info 2")!);

        string[] options = { "OK!" };
        Menu menu = new Menu("", options, Menu.ExitModeEnum.NONE);
        menu.Run();

        ConfigManager.SetValue("initInfo", "1");

        Console.Clear();
    }

    private void RenderLogo()
    {
        Console.Write("  _____ ____ ____    ____                                    \r\n |_   _|  _ \\___ \\  |  _ \\ _ __ ___  ___  ___ _ __   ___ ___ \r\n   | | | | | |__) | | |_) | '__/ _ \\/ __|/ _ \\ '_ \\ / __/ _ \\\r\n   | | | |_| / __/  |  __/| | |  __/\\__ \\  __/ | | | (_|  __/\r\n   |_| |____/_____| |_|   |_|  \\___||___/\\___|_| |_|\\___\\___|\r\n                                         ");
        Console.WriteLine($"v{currentVersion} by Spythere\n\n");
    }

    private void RunMainMenu()
    {     
        Menu menu = new Menu(GetLocaleMenuTitle(), GetLocaleMenuOptions(), Menu.ExitModeEnum.APP);

        do
        {
            RenderLogo();

            int selectedIndex = menu.Run();

            switch (selectedIndex)
            {
                case 0:
                case 1:
                    RunDispatcherOrDriverSelection(selectedIndex);
                    break;
                case 2:
                    RunEditorSelection();
                    break;
                case 3:
                    LanguageManager.RunLanguageChoice(true);

                    menu.SetTitle(GetLocaleMenuTitle());
                    menu.SetOptions(GetLocaleMenuOptions());

                    break;
                case 4:
                    LanguageManager.RunRPCLanguageChoice(true);
                    break;
                default:
                    mainLoop = false;
                    break;
            }

            if (selectedIndex >= 0 && selectedIndex <= 2)
            {
                Console.ReadKey();
                PresenceTimer.Stop();
                PresenceManager.ResetPresenceData();
                Console.Clear();
            }

        } while (mainLoop == true);
    }

    private void RunDispatcherOrDriverSelection(int SelectedIndex)
    {
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
        PresenceTimer.Run(SelectedIndex == 0 ? PresenceMode.DISPATCHER : PresenceMode.DRIVER, username);
    }

    private void RunEditorSelection()
    {
        ConsoleUtils.WritePrompt(ResourceUtils.Get("Scenery Name Prompt")!);
        string? sceneryName = Console.ReadLine();

        while (string.IsNullOrWhiteSpace(sceneryName))
        {
            ConsoleUtils.WritePrompt(ResourceUtils.Get("Scenery Name Prompt")!);
            sceneryName = Console.ReadLine();
        }

        PresenceManager.InitializePresence();
        PresenceManager.ShowPresenceEditorData(sceneryName);
    }

    private string GetLocaleMenuTitle()
    {
        return ResourceUtils.Get("Mode Choice Info").ToUpper();
    }

    private string[] GetLocaleMenuOptions()
    {
        return new string[]
        {
            ResourceUtils.Get("Mode Choice Dispatcher"),
            ResourceUtils.Get("Mode Choice Driver"),
            ResourceUtils.Get("Mode Choice Editor"),
            ResourceUtils.Get("Mode Choice Language"),
            ResourceUtils.Get("Mode Choice RPC Language"),
            ResourceUtils.Get("Mode Choice Exit")
        };
    }

}





