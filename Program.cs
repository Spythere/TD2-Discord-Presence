using System.Globalization;
using System.Reflection;
using TD2_Presence;
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
        CultureInfo.CurrentCulture = new CultureInfo("pl-PL");
    }

    public void SetupProgram()
    {
        /* Presence handlers */
        AppDomain.CurrentDomain.ProcessExit += new EventHandler(PresenceTimer.OnProcessExit!);

        /* Console app title */
        Console.Title = $"{ResourceUtils.Get("Title")} ({currentVersion})";

        /* Update checking */
        Console.WriteLine(ResourceUtils.Get("Update Checking"));
        Task.Run(() => UpdaterUtils.CheckForUpdates()).Wait();
        Console.Clear();

        /* Config file setup */
        ConfigManager.SetupConfig();

        /* Initial messages */
        Console.WriteLine($"==== TD2 Discord Presence (v{currentVersion}) by Spythere ====");
        ConsoleUtils.WriteWarning(ResourceUtils.Get("Initial Info 1")!);
        ConsoleUtils.WriteWarning(ResourceUtils.Get("Initial Info 2")!);
        Console.WriteLine("==========================================");
        Console.WriteLine();
        
        /* Program main loop */
        runMainLoop();
    }

    private void runMainLoop()
    {
        while (mainLoop)
        {
            ConsoleUtils.WritePrompt(ResourceUtils.Get("Mode Choice Info")!);
            ConsoleKey key = Console.ReadKey().Key;
            Console.WriteLine();

            switch (key)
            {
                case ConsoleKey.D1:
                case ConsoleKey.D2:
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
                    PresenceTimer.Run(key == ConsoleKey.D1 ? PresenceMode.DISPATCHER : PresenceMode.DRIVER, username);

                    break;

                case ConsoleKey.D3:
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

                default:
                    System.Environment.Exit(0);
                    break;
            }

            ConsoleUtils.WriteWarning(ResourceUtils.Get("Change Settings Info")!);
            key = Console.ReadKey().Key;

            PresenceTimer.Stop();
            PresenceManager.ResetPresenceData();

            if (key != ConsoleKey.Enter)
                mainLoop = false;
        }
    }
}





