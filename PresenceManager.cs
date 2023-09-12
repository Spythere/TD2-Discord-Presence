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
        static bool timetableFound = false;

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
                    ConsoleUtils.WriteSuccess(ResourceUtils.Get("Presence Loaded Info"));
                }
            };

            ConsoleUtils.WriteInfo(ResourceUtils.Get("Presence Loading Info"));
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

            ConsoleUtils.WriteSuccess(string.Format(ResourceUtils.Get("Scenery Editing Info"), sceneryName));

            rpcClient?.SetPresence(new RichPresence()
            {
                Details = ResourceUtils.Get("RPC Scenery Details Info"),
                State = $"{ResourceUtils.Get("RPC Scenery State Info")}{sceneryName}",
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
                    timetableFound = false;

                    ConsoleUtils.WriteSuccess(string.Format(ResourceUtils.Get("Driver Found Info"), refreshSeconds.ToString()));
                    startTime = DateTime.UtcNow;
                }

                string currentScenery = driverData.currentStationName.Contains(".sc")
                    ? driverData.currentStationName.Split(".sc")[0].Split(" ")[0] + " - offline"
                    : driverData.currentStationName;

                string? connectionTrack = driverData.connectedTrack != "" ? $"/ {ResourceUtils.Get("Current Track Title")} {driverData.connectedTrack.Split("/")[0]}" : null;
                string? connectionSignal = driverData.signal != "" ? $"/ {ResourceUtils.Get("Current Signal Title")} {driverData.signal.Split("/")[0]}" : null;

                string State;
                string Details;

                if (driverData.timetable != null)
                {
                    if(timetableFound == false)
                    {
                            timetableFound = true;
                        DateTime scheduledBegin = DateTimeOffset.FromUnixTimeMilliseconds(driverData.timetable.stopList[0].departureTimestamp).DateTime;

                        startTime = DateTime.Compare(scheduledBegin, DateTime.UtcNow) < 0 ? scheduledBegin : DateTime.UtcNow;
                    }

                    string timetableRoute = driverData.timetable.category
                        + " " + driverData.trainNo
                        + " " + driverData.timetable.route.Replace("|", " -> ");

                    Details = timetableRoute;
                    State = $"{currentScenery} {(connectionTrack ?? connectionSignal)} ({driverData.speed} km/h)";

                    DiscordRPC.Button rjButton = new DiscordRPC.Button()
                    {
                        Label = ResourceUtils.Get("RPC Driver Timetable Button Label"),
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
                            SmallImageText = ResourceUtils.Get("RPC Driver Mode Title")
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
                    if (timetableFound == true)
                    {
                        timetableFound = false;
                        startTime = DateTime.UtcNow;
                    }

                    Details = $"{driverData.trainNo} - {ResourceUtils.Get("RPC Driver No Timetable Title")}";
                    State = $"{currentScenery} {connectionSignal} ({driverData.speed} km/h)";

                    rpcClient?.SetPresence(new RichPresence()
                    {
                        Details = Details,
                        State = State,
                        Assets = new Assets()
                        {
                            LargeImageKey = "largeimage",
                            SmallImageKey = "driver",
                            SmallImageText = ResourceUtils.Get("RPC Driver Mode Title")
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
                timetableFound = false;

                

                ConsoleUtils.WriteWarning(ResourceUtils.Get("Driver Not Found Info"));
                rpcClient?.ClearPresence();
            }
        }

        public static void ShowPresenceDispatcherData(PlayerActivityData? playerActivity, int refreshSeconds)
        {
            IList<DispatcherData>? dispatcherData = playerActivity?.dispatcher;

            if (dispatcherData == null || dispatcherData.Count == 0)
            {
                userFound = false;
                ConsoleUtils.WriteWarning(ResourceUtils.Get("Dispatcher Not Found Info"));
                rpcClient?.ClearPresence();

                return;
            }

            if (userFound == false)
            {
                ConsoleUtils.WriteSuccess(string.Format(ResourceUtils.Get("Dispatcher Found Info"), refreshSeconds.ToString()));
                userFound = true;
                startTime = DateTime.UtcNow;
            }

            DispatcherData currentData = dispatcherData[currentSceneryIndex];
            startTime = DateTimeOffset.FromUnixTimeMilliseconds(currentData.timestampFrom).DateTime;

            RichPresence rp = new RichPresence()
            {
                Details = $"{ResourceUtils.Get("RPC Dispatcher Scenery Title")} {currentData.stationName}",
                State = $"Status: {DispatcherUtils.getDispatcherStatus(currentData.dispatcherStatus)}",
                Assets = new Assets()
                {
                    LargeImageKey = "largeimage",
                    SmallImageKey = "dispatcher",
                    SmallImageText = ResourceUtils.Get("RPC Dispatcher Mode Title")
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
