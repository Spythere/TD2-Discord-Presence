namespace TD2_Presence.Utils
{
    public static class FileUtils
    {
        static readonly string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        static readonly string filePath = $"{docPath}/TD2Presence/data.txt";

        public static void writeDoc(string content)
        {
            if (!Directory.Exists($"{docPath}/TD2Presence"))
                Directory.CreateDirectory($"{docPath}/TD2Presence");

            File.WriteAllText(filePath, content);
        }

        public static string? readDoc()
        {
            if (!File.Exists(filePath)) return null;

            try
            {
                string data = File.ReadAllText(filePath);
                return data;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
