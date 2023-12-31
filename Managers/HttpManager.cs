﻿using System.Net;
using System.Net.Http.Json;
using TD2_Presence.Classes;
using TD2_Presence.Utils;

namespace TD2_Presence.Managers
{
    public static class HttpManager
    {
        static readonly HttpClient client = new HttpClient();

        public static async Task<PlayerActivityData?> FetchPlayerActivityData(string username)
        {
            PlayerActivityData? result = null;

            try
            {
                HttpResponseMessage response = await client.GetAsync($"https://stacjownik.spythere.eu/api/getPlayerActivity?name={username}");

                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadFromJsonAsync<PlayerActivityData>();
                }

                if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    ConsoleUtils.WriteError(ResourceUtils.Get("User Is Blocked Warning"));
                    Console.ReadKey();
                    Environment.Exit(0);
                }
            }
            catch (HttpRequestException)
            {
                ConsoleUtils.WriteError(ResourceUtils.Get("Server Error Warning"));
            }

            return result;
        }
    }
}
