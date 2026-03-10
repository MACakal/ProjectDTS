public static class Menu
{
    public static int ShowMenu(string title, string[] options, bool clear = true)
    {
        int selectedIndex = 0;
        ConsoleKey key;
        int startRow = Console.CursorTop;
        do
        {
            if (clear)
            {
                Console.Clear();
            }
            else
            {
                Console.SetCursorPosition(0, startRow);
            }

            Console.WriteLine(title);
            Console.WriteLine();

            for (int i = 0; i < options.Length; i++)
            {
                if (i == selectedIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("► " + options[i]);
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine("  " + options[i]);
                }
            }

            key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.UpArrow)
            {
                selectedIndex--;
                if (selectedIndex < 0)
                    selectedIndex = options.Length - 1;
            }

            if (key == ConsoleKey.DownArrow)
            {
                selectedIndex++;
                if (selectedIndex >= options.Length)
                    selectedIndex = 0;
            }

        } while (key != ConsoleKey.Enter);

        return selectedIndex;
    }
}