using DiscordRPC.Logging;
using DiscordRPC;
using System.Reflection.Emit;
using TD2_Presence;


TD2Presence TD2Presence = new TD2Presence();
AppDomain.CurrentDomain.ProcessExit += new EventHandler(TD2Presence.OnProcessExit);

Console.Write("Wpisz nazwę użytkownika: ");
string? username = Console.ReadLine();

while(username == null)
{
    Console.WriteLine("Wpisz poprawny nick!");
    Console.Write("Wpisz nazwę użytkownika: ");
    username = Console.ReadLine();
}

PresenceManager.InitializePresence();
TD2Presence.RunTimer(username);

Console.WriteLine("Aby wyjść naciśnij dowolny klawisz...");
Console.ReadLine();

public class TD2Presence
{
    public void RunTimer(string username)
    {
        UpdateDriverData(username);

        var timer = new System.Timers.Timer(10000);

        timer.Elapsed += (sender, args) =>
        {
            UpdateDriverData(username);
        };

        timer.Start();
    }

    public async void UpdateDriverData(string username)
    {
        Console.WriteLine("Dane są pobierane...");

        ActiveTrain? train = await APIHandler.FetchTrainData(username);
        PresenceManager.ShowPresenceDriverData(train, 15);

    }

    public void OnProcessExit(object sender, EventArgs e)
    {
        PresenceManager.ShutdownPresence();
    }
}

