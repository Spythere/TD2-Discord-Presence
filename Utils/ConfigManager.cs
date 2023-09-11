using Newtonsoft.Json;

namespace TD2_Presence.Utils
{
    public static class ConfigManager
    {
        static readonly string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        static readonly string configPath = $"{docPath}/TD2Presence/config.json";
           
        public static void SetupConfig()
        {
            if (!Directory.Exists($"{docPath}/TD2Presence"))
            {
                Directory.CreateDirectory($"{docPath}/TD2Presence");
                /*string output = Newtonsoft.Json.JsonConvert.SerializeObject(null, Newtonsoft.Json.Formatting.Indented);*/
                File.WriteAllText(configPath, "{}");
            }
        }

        public static void SetValue(string key, string content)
        {
            if (!File.Exists(configPath)) return;

            string json = File.ReadAllText(configPath);
            dynamic jsonObj = JsonConvert.DeserializeObject(json);
            jsonObj[key] = content;
            string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            File.WriteAllText(configPath, output);
        }

        public static string? ReadValue(string key)
        {
            if (!File.Exists(configPath)) return null;

            string json = File.ReadAllText(configPath);
            dynamic jsonObj = JsonConvert.DeserializeObject(json);

            return jsonObj[key];
        }

    }
}
