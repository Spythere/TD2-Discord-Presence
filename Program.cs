using System.Reflection;
using TD2_Presence;
using TD2_Presence.Utils;

PresenceProgram presenceProgram = new PresenceProgram();
presenceProgram.SetupProgram();

class PresenceProgram
{
    Version? currentVersion;
    bool mainLoop = true;

    public PresenceProgram() { 
        currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
    }

    public void SetupProgram()
    {
        /* Presence handlers */
        AppDomain.CurrentDomain.ProcessExit += new EventHandler(PresenceTimer.OnProcessExit!);

        /* Console app title */
        Console.Title = $"TD2 Discord Presence ({currentVersion})";

        /* Update checking */
        Console.WriteLine("Sprawdzanie aktualizacji...");
        Task.Run(() => UpdaterUtils.CheckForUpdates()).Wait();
        Console.Clear();

        /* Config file setup */
        ConfigManager.SetupConfig();

        /* Initial messages */
        Console.WriteLine($"==== TD2 Discord Presence (v{currentVersion}) by Spythere ====");
        ConsoleUtils.WriteWarning("Upewnij się, że masz włączoną oryginalną desktopową aplikację Discorda, a następnie wpisz dane poniżej. Aktywność zniknie automatycznie po zamknięciu tego okna terminalu. Miłego korzystania!");
        ConsoleUtils.WriteWarning("Pamiętaj, aby włączyć wyświetlanie statusu rozgrywki w ustawieniach aktywności Discorda ORAZ indywidualnych ustawieniach prywatności serwera, na którym chcesz je pokazać!");
        Console.WriteLine("==========================================");
        Console.WriteLine();
        
        /* Program main loop */
        runMainLoop();
    }

    private void runMainLoop()
    {
        while (mainLoop)
        {
            ConsoleUtils.WritePrompt("Wybierz tryb (1 - dyżurny; 2 - maszynista; 3 - edytor; inne - wyjście): ");
            ConsoleKey key = Console.ReadKey().Key;
            Console.WriteLine();

            switch (key)
            {
                case ConsoleKey.D1:
                case ConsoleKey.D2:
                    string? savedUsername = ConfigManager.ReadValue("savedUsername");

                    if (savedUsername != null)
                        ConsoleUtils.WritePrompt($"Wpisz nazwę użytkownika (domyślnie: {savedUsername}): ");
                    else
                        ConsoleUtils.WritePrompt("Wpisz nazwę użytkownika: ");

                    string? username = Console.ReadLine();

                    if (string.IsNullOrEmpty(savedUsername))
                    {
                        while (string.IsNullOrWhiteSpace(username))
                        {
                            ConsoleUtils.WriteWarning("Wpisz poprawny nick!");
                            ConsoleUtils.WritePrompt("Wpisz nazwę użytkownika: ");
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
                    ConsoleUtils.WritePrompt($"Wpisz nazwę scenerii: ");
                    string? sceneryName = Console.ReadLine();

                    while (string.IsNullOrWhiteSpace(sceneryName))
                    {
                        ConsoleUtils.WritePrompt($"Wpisz nazwę scenerii: ");
                        sceneryName = Console.ReadLine();
                    }

                    PresenceManager.InitializePresence();
                    PresenceManager.ShowPresenceEditorData(sceneryName);

                    break;

                default:
                    System.Environment.Exit(0);
                    break;
            }

            ConsoleUtils.WriteWarning("Aby zmienić ustawienia naciśnij Enter, aby wyjść dowolny inny klawisz");
            key = Console.ReadKey().Key;

            PresenceTimer.Stop();
            PresenceManager.ResetPresenceData();

            if (key != ConsoleKey.Enter)
                mainLoop = false;
        }
    }
}







