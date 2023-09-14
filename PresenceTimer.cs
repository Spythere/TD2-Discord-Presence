using TD2_Presence.Classes;
using TD2_Presence.Managers;
using TD2_Presence.Utils;
using Timer = System.Timers.Timer;

namespace TD2_Presence
{
    public enum PresenceMode
    {
        DISPATCHER = 1,
        DRIVER = 2,
        EDITOR = 3
    };

    public static class PresenceTimer
    {
        public static readonly int refreshSeconds = 15;
        public static Timer? timer;

        public static void Run(PresenceMode mode, string username)
        {
            ConsoleUtils.WriteInfo(string.Format(ResourceUtils.Get("Searching User Info"), username));
            RunUpdate(mode, username);

            timer = new Timer(refreshSeconds * 1000);

            timer.Elapsed += (sender, args) =>
            {
                RunUpdate(mode, username);
            };

            timer.Start();
        }

        public static void Stop()
        {
            timer?.Stop();
        }

        private static async void RunUpdate(PresenceMode mode, string username)
        {
            PlayerActivityData? playerActivity = await HttpManager.FetchPlayerActivityData(username);

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

        public static void OnProcessExit(object sender, EventArgs e)
        {
            PresenceManager.ShutdownPresence();
        }
    }
}
