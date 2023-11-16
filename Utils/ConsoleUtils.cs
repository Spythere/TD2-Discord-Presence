namespace TD2_Presence.Utils
{
    public abstract class ConsoleUtils
    {
        public static void WriteInfo(string s)
        {
            Console.WriteLine($"> {s}");
        }

        public static void WritePrompt(string s)
        {
            Console.Write($"> {s}");
        }

        public static void WriteSuccess(string s)
        {
            Console.WriteLine($"> {s}");
        }

        public static void WriteError(string s)
        {
            Console.WriteLine($"<!!> {s}");
        }

        public static void WriteWarning(string s)
        {
            Console.WriteLine($"<!> {s}");
        }

        public static void ResetLine()
        {
            Console.WriteLine(new string(' ', Console.WindowWidth));
        }
    }
}
