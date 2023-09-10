using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using TD2_Presence.Classes;
using TD2_Presence.Utils;

namespace TD2_Presence
{
    public static class APIHandler
    {
        static readonly HttpClient client = new HttpClient();

        public static async Task<PlayerActivityData?> FetchPlayerActivityData(string username)
        {
            PlayerActivityData? result = null;
            try
            {
                HttpResponseMessage response = await client.GetAsync($"https://stacjownik.spythere.pl/api/getPlayerActivity?name={username}");

                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadFromJsonAsync<PlayerActivityData>();
                }

                if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    ConsoleUtils.WriteError("Ten użytkownik jest zablokowany!");
                    Console.ReadKey();
                    System.Environment.Exit(0);
                }
            }
            catch (HttpRequestException)
            {
                ConsoleUtils.WriteError("Wystąpił błąd podczas łączenia z serwerem!");
            }
           
            return result;
        }
    }
}
