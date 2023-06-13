using DiscordRPC;
using DiscordRPC.Logging;
using TD2_Presence.Classes;
using TD2_Presence.Utils;

namespace TD2_Presence
{
    static class PresenceManager
    {
        static DiscordRpcClient? rpcClient;
        static DateTime startTime = DateTime.UtcNow;
        static bool userFound = false;
        static int currentSceneryIndex = 0;

        public static void InitializePresence()
        {
            if (rpcClient != null) return;

            rpcClient = new("1080201895139885066")
            {
                Logger = new ConsoleLogger() { Level = LogLevel.Warning }
            };

            rpcClient.OnReady += (sender, e) =>
            {
                Console.WriteLine("[V] Discord Presence gotowy!");
            };

            Console.WriteLine("[!] Łączenie z Discordem...");
            rpcClient?.Initialize();
        }

        public static void ShutdownPresence()
        {
            rpcClient?.Dispose();
        }

        public static void ResetPresenceData()
        {
            rpcClient?.ClearPresence();

            currentSceneryIndex = 0;
            userFound = false;
            startTime = DateTime.UtcNow;
        }

        public static void ShowPresenceEditorData(string sceneryName)
        {
            startTime = DateTime.UtcNow;

            Console.WriteLine($"[V] Wyświetlanie statusu edycji scenerii " + sceneryName + "!");

            rpcClient?.SetPresence(new RichPresence()
            {
                Details = "W edytorze scenerii",
                State = "Edytuje scenerię: " + sceneryName,
                Assets = new Assets()
                {
                    LargeImageKey = "largeimage",
                },
                Timestamps = new Timestamps()
                {
                    Start = startTime,
                }
            });
        }

        public static void ShowPresenceDriverData(ActiveTrain? trainData, int refreshSeconds)
        {
            if (trainData != null)
            {
                if (userFound == false)
                {
                    userFound = true;
                    Console.WriteLine($"[V] Znaleziono maszynistę! Dane będą odświeżać się co {refreshSeconds}s!");
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
                    startTime = DateTimeOffset.FromUnixTimeMilliseconds(trainData.timetable.stopList[0].departureTimestamp).DateTime;

                    Button rjButton = new Button()
                    {
                        Label = "Szczegóły rozkładu jazdy",
                        Url = $"https://stacjownik-td2.web.app/trains?trainId={trainData.driverName + trainData.trainNo.ToString()}"
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

                Console.WriteLine("[!] Nie znaleziono maszynisty online! Oczekiwanie...");
                rpcClient?.ClearPresence();
            }
        }

        public static void ShowPresenceDispatcherData(IList<DispatcherData>? data, int refreshSeconds)
        {
            if (data == null || data.Count == 0)
            {
                userFound = false;

                Console.WriteLine("[!] Nie znaleziono dyżurnego, oczekiwanie...");
                rpcClient?.ClearPresence();

                return;
            }

            if (userFound == false)
            {
                Console.WriteLine("[V] Znaleziono dyżurnego! Dane będą się odświeżać co " + refreshSeconds + "s!");
                userFound = true;
                startTime = DateTime.UtcNow;
            }

            DispatcherData currentData = data[currentSceneryIndex];
            startTime = DateTimeOffset.FromUnixTimeMilliseconds(currentData.timestampFrom).DateTime;

            RichPresence rp = new RichPresence()
            {
                Details = $"Dyżurny na scenerii {currentData.stationName}",
                State = $"Status: {DispatcherUtils.getDispatcherStatus(currentData.dispatcherStatus)}",
                Assets = new Assets()
                {
                    LargeImageKey = "largeimage",
                    SmallImageKey = "dispatcher",
                    SmallImageText = "Tryb dyżurnego"
                },
                Timestamps = new Timestamps()
                {
                    Start = startTime,
                }
            };

            if (data.Count > 1)
            {
                rp.Party = new Party
                {
                    ID = Secrets.CreateFriendlySecret(new Random()),
                    Size = currentSceneryIndex + 1
                    ,
                    Max = data.Count,
                    Privacy = Party.PrivacySetting.Public
                };
            }

            rpcClient?.SetPresence(rp);

            currentSceneryIndex = (currentSceneryIndex + 1) % data.Count;
        }
    }
}
