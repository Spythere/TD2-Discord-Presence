﻿using System.Diagnostics;
using System.Net.Http.Json;
using System.Reflection;
using TD2_Presence.Classes;
using Timer = System.Timers.Timer;

namespace TD2_Presence.Utils
{
    public abstract class UpdaterUtils
    {
        static readonly HttpClient client = new HttpClient() {
            Timeout = TimeSpan.FromSeconds(5),
        };


        public async static Task<bool> CheckForUpdates()
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36");

            try
            {
                HttpResponseMessage response = await client.GetAsync("https://api.github.com/repos/Spythere/TD2-Discord-Presence/releases/latest");

                if(response == null || !response.IsSuccessStatusCode) 
                {
                    return false;
                }

                GithubAPIReleaseData? data = await response.Content.ReadFromJsonAsync<GithubAPIReleaseData>();

                if (data == null)
                {
                    return false;
                }

                Version? currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
                Version latestVersion = new Version(data.TagName);

                if (latestVersion > currentVersion)
                {
                    Console.Clear();
                    ConsoleUtils.WriteWarning(string.Format(ResourceUtils.Get("Update Dialog Desc"), latestVersion));

                    Menu menu = new Menu("", new string[]{ "OK!", "Cancel" }, Menu.ExitModeEnum.NONE);

                 // DialogResult dialogResult = MessageBox.Show( ResourceUtils.Get("Update Dialog Title"), MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    int selectedIndex = menu.Run();

                    if(selectedIndex == 0)
                    {
                        string downloadUrl = data.Assets[0].BrowserDownloadUrl;

                        Console.Clear();
                        ConsoleUtils.WriteWarning(ResourceUtils.Get("Update Info 1"));
                        ConsoleUtils.WriteWarning(ResourceUtils.Get("Update Info 2"));
                        Console.WriteLine();
                        Console.WriteLine(downloadUrl);

                        Process.Start(new ProcessStartInfo(downloadUrl) { UseShellExecute = true });

                        Console.WriteLine();
                        ConsoleUtils.WriteWarning(ResourceUtils.Get("Update Info 3"));

                        Menu menuExit = new Menu("", new string[] { "OK!" }, Menu.ExitModeEnum.NONE);
                        menuExit.Run();

                        System.Environment.Exit(0);
                    }       
                }
            }
            catch (Exception)
            {
                ConsoleUtils.WriteError(ResourceUtils.Get("Update Error"));
            }

            return false;
        }
    }
}
