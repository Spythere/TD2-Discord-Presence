using DiscordRPC;
using DiscordRPC.Logging;

namespace TD2_Presence
{
    static class PresenceManager
    {
        static DiscordRpcClient? rpcClient;
        static DateTime startTime = DateTime.Now;
        static bool userFound = false;

        public static void InitializePresence()
        {
            rpcClient = new("1080201895139885066")
            {
                Logger = new ConsoleLogger() { Level = LogLevel.Warning }
            };

            rpcClient.OnReady += (sender, e) =>
            {
                Console.WriteLine("Discord Presence gotowy dla użytkownika: {0}", e.User.Username);
            };

            rpcClient?.Initialize();
        }

        public static void ShutdownPresence()
        {
            rpcClient?.Dispose();
        }

        public static void ShowPresenceDriverData(ActiveTrain? trainData, int refreshSeconds)
        {

            if (trainData != null)
            {
                if (userFound == false)
                {
                    userFound = true;
                    Console.WriteLine($"Znaleziono maszynistę! Dane będą odświeżać się co {refreshSeconds}s!");
                }

                string currentScenery = trainData.currentStationName.Contains(".sc")
                    ? trainData.currentStationName.Split(".sc")[0].Split(" ")[0] + " - offline"
                    : trainData.currentStationName;

                string connectionTrack = trainData.connectedTrack != "" ? "/ szlak " + trainData.connectedTrack.Split("/")[0] : "";
                string connectionSignal = trainData.signal != "" ? "/ semafor " + trainData.signal.Split("/")[0] : "";

                string State;
                string Details;

                if (trainData.timetable != null)
                {
                    string timetableRoute = trainData.timetable.category
                        + " " + trainData.trainNo
                        + " " + trainData.timetable.route.Replace("|", " -> ");

                    Details = timetableRoute;
                    State = $"{currentScenery} {connectionTrack} ({trainData.speed} km/h)";
                    startTime = DateTimeOffset.FromUnixTimeMilliseconds(trainData.timetable.stopList[0].departureTimestamp).DateTime; ;

                    Button rjButton = new Button()
                    {
                        Label = "Szczegóły rozkładu jazdy",
                        Url = $"https://stacjownik-td2.web.app/trains?train={trainData.trainNo}"
                    };

                    rpcClient?.SetPresence(new RichPresence()
                    {
                        Details = Details,
                        State = State,
                        Assets = new Assets()
                        {
                            LargeImageKey = "largeimage",
                            SmallImageKey = "driver",
                            SmallImageText = "Tryb maszynisty"
                        },
                        Timestamps = new Timestamps()
                        {
                            Start = startTime,
                        },
                        Buttons = new Button[]
                        {
                            rjButton
                        }
                    });
                }
                else
                {
                    Details = "Brak rozkładu jazdy";
                    State = $"{currentScenery} {connectionSignal} ({trainData.speed} km/h)";

                    rpcClient?.SetPresence(new RichPresence()
                    {
                        Details = Details,
                        State = State,
                        Assets = new Assets()
                        {
                            LargeImageKey = "largeimage",
                            SmallImageKey = "driver",
                            SmallImageText = "Tryb maszynisty"
                        },
                        Timestamps = new Timestamps()
                        {
                            Start = startTime,
                        }
                    });
                }


            }
            else
            {
                userFound = false;

                Console.WriteLine("Nie znaleziono maszynisty online! Oczekiwanie...");
                rpcClient?.ClearPresence();
            }
        }


    }
}
