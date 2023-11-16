using TD2_Presence.Utils;

namespace TD2_Presence
{
    public class Menu
    {
        public enum ExitModeEnum
        {
            APP = 0,
            /* MENU = 1, WIP */
            NONE = 2,
        }


        private int SelectedIndex;
        private string[] Options;
        private string Title;
        private readonly int SpaceSpan = 2;
        private ExitModeEnum ExitMode;
        private bool Running = true;

        public Menu(string title, string[] options, ExitModeEnum exitMode)
        {
            SelectedIndex = 0;
            Options = options;
            Title = title;
            ExitMode = exitMode;
        }

        public void SetOptions(string[] options) { 
            Options = options;
        }

        public void SetTitle(string title)
        {
            Title = title;
        }

        public void DisplayOptions()
        {
            Console.WriteLine(new string(' ', 2) + Title);
            Console.WriteLine();

            for (int i = 0; i < Options.Length; i++)
            {
                string prefix = SelectedIndex == i ? "*" : " ";   
                Console.ForegroundColor = SelectedIndex == i ? ConsoleColor.Black : ConsoleColor.White;
                Console.BackgroundColor = SelectedIndex == i ? ConsoleColor.White : ConsoleColor.Black;

                Console.WriteLine($"{ prefix } << {Options[i]} >>");
            }

            Console.ResetColor();
        }

        private void ExitMenu()
        {
            switch (ExitMode)
            {
                case ExitModeEnum.NONE:
                    return;
                /*case ExitModeEnum.MENU:
                    Running = false;
                    break;*/
                case ExitModeEnum.APP:
                default:
                    System.Environment.Exit(0); 
                    break;
            }
        }

        public int Run()
        {
            ConsoleKey keyPressed;
            Running = true;

            do
            {
                DisplayOptions();

                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                keyPressed = keyInfo.Key;

                switch (keyPressed)
                {
                    case ConsoleKey.UpArrow:
                    case ConsoleKey.W:
                        SelectedIndex--;
                        break;

                    case ConsoleKey.DownArrow:
                    case ConsoleKey.S:
                        SelectedIndex++;
                        break;
                    case ConsoleKey.Escape:
                        ExitMenu();
                        break;
                    default:
                        break;
                }
                
                SelectedIndex = SelectedIndex == -1 ? Options.Length - 1 : SelectedIndex % Options.Length;

                Console.SetCursorPosition(0, Console.GetCursorPosition().Top - Options.Length - SpaceSpan);

            } while (keyPressed != ConsoleKey.Enter && Running);
                
            for (int i = 0; i < Options.Length + SpaceSpan; i++)
            {
                ConsoleUtils.ResetLine();
            }

            Console.SetCursorPosition(0, Console.GetCursorPosition().Top - Options.Length - SpaceSpan);

            return SelectedIndex;
        }
    }
}
