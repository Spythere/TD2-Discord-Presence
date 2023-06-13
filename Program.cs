using DiscordRPC.Logging;
using DiscordRPC;
using System.Reflection.Emit;
using TD2_Presence;
using TD2_Presence.Classes;
using TD2_Presence.Utils;
using System.Timers;
using Timer = System.Timers.Timer;

TD2Presence TD2Presence = new TD2Presence();
AppDomain.CurrentDomain.ProcessExit += new EventHandler(TD2Presence.OnProcessExit);
bool mainLoop = true;

Console.WriteLine("==== TD2 Discord Presence by Spythere ====");
Console.WriteLine("[!] Upewnij się, że masz włączoną oryginalną desktopową aplikację Discorda, a następnie wpisz dane poniżej. Aktywność zniknie automatycznie po zamknięciu tego okna terminalu. Miłego korzystania!");
Console.WriteLine("[!] Pamiętaj, aby włączyć wyświetlanie statusu rozgrywki w ustawieniach aktywności Discorda ORAZ indywidualnych ustawieniach prywatności serwera, na którym chcesz je pokazać!");
Console.WriteLine();

while (mainLoop)
{
    Console.Write("[?] Wybierz tryb: Dyżurny [1] / Maszynista [2] / Edytor [3] / Wyjście [inny wybór]: ");
    string? mode = Console.ReadLine();

    switch (mode)
    {
        case "1":
        case "2":
            string? savedUsername = FileUtils.readDoc();

            if (savedUsername != null)
                Console.Write($"[?] Wpisz nazwę użytkownika (domyślnie: {savedUsername}): ");
            else
                Console.Write("[?] Wpisz nazwę użytkownika: ");

            string? username = Console.ReadLine();

            if (string.IsNullOrEmpty(savedUsername))
            {
                while (string.IsNullOrWhiteSpace(username))
                {
                    Console.WriteLine("[!] Wpisz poprawny nick!");
                    Console.Write("[?] Wpisz nazwę użytkownika: ");
                    username = Console.ReadLine();
                }
            }
            else
                username = string.IsNullOrWhiteSpace(username) ? savedUsername : username;

            FileUtils.writeDoc(username);

            PresenceManager.InitializePresence();
            TD2Presence.RunTimer((PresenceMode)Enum.Parse(typeof(PresenceMode), mode), username);

            break;

        case "3":
            Console.Write("[?] Wpisz nazwę scenerii: ");
            string? sceneryName = Console.ReadLine();

            while (string.IsNullOrWhiteSpace(sceneryName))
            {
                Console.Write("[?] Wpisz nazwę scenerii: ");
                sceneryName = Console.ReadLine();
            }

            PresenceManager.InitializePresence();
            PresenceManager.ShowPresenceEditorData(sceneryName);

            break;

        default:
            System.Environment.Exit(0);
            break;
    }

    Console.WriteLine("[!] Aby zmienić ustawienia naciśnić Enter, aby wyjść dowolny inny klawisz");
    ConsoleKeyInfo key = Console.ReadKey();
    
    TD2Presence.StopTimer();
    PresenceManager.ResetPresenceData();

    // Wciśnięty enter
    if (key.KeyChar != (char)13)
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
        Console.WriteLine("[!] Szukanie informacji o graczu " + username + "...");
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

    public void RunUpdate(PresenceMode mode, string username)
    {
        switch (mode)
        {
            case PresenceMode.DRIVER:
                UpdateDriverData(username);
                break;
            case PresenceMode.DISPATCHER:
                UpdateDispatcherData(username);
                break;
        }
    }

    public async void UpdateDriverData(string username)
    {
        ActiveTrain? train = await APIHandler.FetchTrainData(username);
        PresenceManager.ShowPresenceDriverData(train, refreshSeconds);
    }

    public async void UpdateDispatcherData(string username)
    {
        IList<DispatcherData>? data = await APIHandler.FetchDispatcherData(username);
        PresenceManager.ShowPresenceDispatcherData(data, refreshSeconds);
    }

    public void OnProcessExit(object sender, EventArgs e)
    {
        PresenceManager.ShutdownPresence();
    }
}

