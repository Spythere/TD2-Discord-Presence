using System.Net.Http.Json;

namespace TD2_Presence
{
    public static class APIHandler
    {
        static readonly HttpClient client = new HttpClient();

        public static async Task<ActiveTrain?> FetchTrainData(string nickname)
        {
            ActiveTrain? trainData = null;

            HttpResponseMessage response = await client.GetAsync("https://spythere.pl/api/getActiveTrainList");
            
            if (response.IsSuccessStatusCode)
            {
                IList<ActiveTrain>? activeTrains = await response.Content.ReadFromJsonAsync<List<ActiveTrain>>();

                trainData = activeTrains?.FirstOrDefault(t => t.driverName.ToLower() == nickname.ToLower());
            }

            return trainData;
        }

        public static async Task<DispatcherData?> FetchDispatcherData(string nickname)
        {
            DispatcherData? dispatcherData = null;

            HttpResponseMessage response = await client.GetAsync($"https://spythere.pl/api/getDispatchers?dispatcherName={nickname}&online=1");

            if (response.IsSuccessStatusCode)
            {
                IList<DispatcherData>? dispatchersResponse = await response.Content.ReadFromJsonAsync<IList<DispatcherData>>();

                dispatcherData = dispatchersResponse == null ? null : dispatchersResponse[0];
            }

            return dispatcherData;
        }
    }
}
