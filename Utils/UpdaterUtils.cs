using System.Diagnostics;
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

        public async static Task<bool> CheckForUpdates()
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Safari/537.36");

            try
            {
                HttpResponseMessage response = await client.GetAsync("https://api.github.com/repos/Spythere/test/releases/latest");

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

                Console.WriteLine(latestVersion.ToString());

                if (latestVersion > currentVersion)
                {
                    DialogResult dialogResult = MessageBox.Show($"Nowa wersja aplikacji ({ latestVersion}) jest dostępna! Czy chcesz ją teraz pobrać?", "Dostępna nowa wersja", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                    
                    if (dialogResult == DialogResult.OK)
                    {
                        string downloadUrl = data.Assets[0].BrowserDownloadUrl;

                        Console.Clear();
                        ConsoleUtils.WriteWarning("W tym momencie twoja domyślna przeglądarka powinna pobrać paczkę ZIP z najnowszą wersją aplikacji.");
                        ConsoleUtils.WriteWarning("Jeśli nic się nie otworzyło, skopiuj i otwórz ręcznie poniższy link, a następnie wypakuj go w dowolnym miejscu.");
                        Console.WriteLine(downloadUrl);

                        Process.Start(new ProcessStartInfo(downloadUrl) { UseShellExecute = true });
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
