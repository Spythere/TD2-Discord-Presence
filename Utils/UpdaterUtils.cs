﻿using System.Diagnostics;
using System.Net.Http.Json;
using System.Reflection;
using TD2_Presence.Classes;

namespace TD2_Presence.Utils
{
    public abstract class UpdaterUtils
    {
        static readonly HttpClient client = new HttpClient() { 
            Timeout = TimeSpan.FromSeconds(5),
        };

        public async static Task<bool> checkForUpdates()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync("https://spythere.github.io/api/discord-presence/data.json");

                if(response == null || !response.IsSuccessStatusCode) 
                {
                    return false;
                }

                GithubAPIData? data = await response.Content.ReadFromJsonAsync<GithubAPIData>();

                if (data == null)
                {
                    return false;
                }

                Version? currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
                Version latestVersion = new Version(data.latestVersion);

                if (latestVersion > currentVersion)
                {
                    DialogResult dialogResult = MessageBox.Show($"Nowa wersja aplikacji ({ latestVersion}) jest dostępna! Czy chcesz ją teraz pobrać?", "Dostępna nowa wersja", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    
                    if (dialogResult == DialogResult.OK)
                    {
                        string uri = "https://spythere.github.io/api/discord-presence/download/TD2_Presence_latest.zip";

                        Console.Clear();
                        ConsoleUtils.WriteWarning("W tym momencie twoja domyślna przeglądarka powinna pobrać paczkę ZIP z najnowszą wersją aplikacji.");
                        ConsoleUtils.WriteWarning("Jeśli nic się nie otworzyło, skopiuj i otwórz ręcznie poniższy link, a następnie wypakuj go w dowolnym miejscu.");
                        Console.WriteLine(uri);

                        Process.Start(new ProcessStartInfo(uri) { UseShellExecute = true });
                        ConsoleUtils.WriteWarning("Naciśnij jakikolwiek przycisk, aby zamknąć aplikację!");
                        
                        Console.ReadKey();
                        System.Environment.Exit(0);
                    }
                }
            }
            catch (Exception)
            {
                ConsoleUtils.WriteError("Ups! Nie można pobrać danych o aktualizacji!");
            }

            return false;
        }
    }
}