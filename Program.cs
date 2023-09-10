using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using TD2_Presence;
using TD2_Presence.Classes;
using TD2_Presence.Utils;
using Timer = System.Timers.Timer;

Version? currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
Console.Title = $"TD2 Discord Presence ({currentVersion})";

TD2Presence TD2Presence = new TD2Presence();
AppDomain.CurrentDomain.ProcessExit += new EventHandler(TD2Presence.OnProcessExit);
bool mainLoop = true;

Console.WriteLine("Sprawdzanie aktualizacji...");
await UpdaterUtils.checkForUpdates();  
Console.Clear();

Console.WriteLine($"==== TD2 Discord Presence (v{currentVersion}) by Spythere ====");
ConsoleUtils.WriteWarning("Upewnij się, że masz włączoną oryginalną desktopową aplikację Discorda, a następnie wpisz dane poniżej. Aktywność zniknie automatycznie po zamknięciu tego okna terminalu. Miłego korzystania!");
ConsoleUtils.WriteWarning("Pamiętaj, aby włączyć wyświetlanie statusu rozgrywki w ustawieniach aktywności Discorda ORAZ indywidualnych ustawieniach prywatności serwera, na którym chcesz je pokazać!");
Console.WriteLine("==========================================");
Console.WriteLine();

while (mainLoop)
{
    ConsoleUtils.WritePrompt("Wybierz tryb (1 - dyżurny; 2 - maszynista; 3 - edytor; inne - wyjście): ");
    ConsoleKey key = Console.ReadKey().Key;
    Console.WriteLine();
        
    switch (key)
    {
        case ConsoleKey.D1:
        case ConsoleKey.D2:
            string? savedUsername = FileUtils.readDoc();

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

            FileUtils.writeDoc(username);

            PresenceManager.InitializePresence();

            TD2Presence.RunTimer(key == ConsoleKey.D1 ? PresenceMode.DISPATCHER : PresenceMode.DRIVER, username);

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
    
    TD2Presence.StopTimer();
    PresenceManager.ResetPresenceData();

    if (key != ConsoleKey.Enter)
        mainLoop = false;       
}

enum PresenceMode
{
    DISPATCHER = 1,
    DRIVER = 2,
    EDITOR = 3
};

class TD2Presence
{
    readonly int refreshSeconds = 15;
    Timer? timer;

    public void RunTimer(PresenceMode mode, string username)
    {
        ConsoleUtils.WriteInfo("Szukanie informacji o graczu " + username + "...");
        RunUpdate(mode, username);

        timer = new Timer(refreshSeconds * 1000);

        timer.Elapsed += (sender, args) =>
        {
            RunUpdate(mode, username);
        };

        timer.Start();
    }

    public void StopTimer()
    {
        timer?.Stop();
    }

    public async void RunUpdate(PresenceMode mode, string username)
    {
        PlayerActivityData? playerActivity = await APIHandler.FetchPlayerActivityData(username);

        switch (mode)
        {
            case PresenceMode.DRIVER:
                PresenceManager.ShowPresenceDriverData(playerActivity, refreshSeconds);
                break;
            case PresenceMode.DISPATCHER:
                PresenceManager.ShowPresenceDispatcherData(playerActivity, refreshSeconds);
                break;
        }
    }

    public void OnProcessExit(object sender, EventArgs e)
    {
        PresenceManager.ShutdownPresence();
    }
}

