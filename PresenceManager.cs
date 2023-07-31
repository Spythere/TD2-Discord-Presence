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
                if(userFound)
                {
                    ConsoleUtils.WriteSuccess("Discord Presence gotowy!");
                }
            };

            ConsoleUtils.WriteInfo("Łączenie z Discordem...");
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

            ConsoleUtils.WriteSuccess($"Wyświetlanie statusu edycji scenerii " + sceneryName + "!");

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

        public static void ShowPresenceDriverData(PlayerActivityData? playerActivity, int refreshSeconds)
        {
            DriverData? driverData = playerActivity?.driver;

            if (driverData != null)
            {
                if (userFound == false)
                {
                    userFound = true;
                    ConsoleUtils.WriteSuccess($"Znaleziono maszynistę! Dane będą odświeżać się co {refreshSeconds}s!");
                    startTime = DateTime.UtcNow;
                }

                string currentScenery = driverData.currentStationName.Contains(".sc")
                    ? driverData.currentStationName.Split(".sc")[0].Split(" ")[0] + " - offline"
                    : driverData.currentStationName;

                string connectionTrack = driverData.connectedTrack != "" ? "/ szlak " + driverData.connectedTrack.Split("/")[0] : "";
                string connectionSignal = driverData.signal != "" ? "/ semafor " + driverData.signal.Split("/")[0] : "";

                string State;
                string Details;

                if (driverData.timetable != null)
                {
                    string timetableRoute = driverData.timetable.category
                        + " " + driverData.trainNo
                        + " " + driverData.timetable.route.Replace("|", " -> ");

                    Details = timetableRoute;
                    State = $"{currentScenery} {connectionTrack} ({driverData.speed} km/h)";

                    DiscordRPC.Button rjButton = new DiscordRPC.Button()
                    {
                        Label = "Szczegóły rozkładu jazdy",
                        Url = $"https://stacjownik-td2.web.app/trains?trainId={driverData.driverName + driverData.trainNo.ToString()}"
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
                        Buttons = new DiscordRPC.Button[]
                        {
                            rjButton
                        }
                    });
                }
                else
                {
                    Details = $"{driverData.trainNo} - brak rozkładu jazdy";
                    State = $"{currentScenery} {connectionSignal} ({driverData.speed} km/h)";

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

                ConsoleUtils.WriteWarning("Nie znaleziono maszynisty online! Oczekiwanie...");
                rpcClient?.ClearPresence();
            }
        }

        public static void ShowPresenceDispatcherData(PlayerActivityData? playerActivity, int refreshSeconds)
        {
            IList<DispatcherData>? dispatcherData = playerActivity?.dispatcher;

            if (dispatcherData == null || dispatcherData.Count == 0)
            {
                userFound = false;

                ConsoleUtils.WriteWarning("Nie znaleziono dyżurnego, oczekiwanie...");
                rpcClient?.ClearPresence();

                return;
            }

            if (userFound == false)
            {
                ConsoleUtils.WriteSuccess("Znaleziono dyżurnego! Dane będą się odświeżać co " + refreshSeconds + "s!");
                userFound = true;
                startTime = DateTime.UtcNow;
            }

            DispatcherData currentData = dispatcherData[currentSceneryIndex];
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

            if (dispatcherData.Count > 1)
            {
                rp.Party = new Party
                {
                    ID = Secrets.CreateFriendlySecret(new Random()),
                    Size = currentSceneryIndex + 1
                    ,
                    Max = dispatcherData.Count,
                    Privacy = Party.PrivacySetting.Public
                };
            }

            rpcClient?.SetPresence(rp);

            currentSceneryIndex = (currentSceneryIndex + 1) % dispatcherData.Count;
        }
    }
}
