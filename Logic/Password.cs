public static class Password
{
    public static string ReadPassword()
    {
        string password = "";
        ConsoleKeyInfo key = new();

        while (true)
        {
            key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Enter)
                break;

            if (key.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password = password[..^1];
                Console.Write("\b \b");
            }
            else if (!char.IsControl(key.KeyChar))
            {
                password += key.KeyChar;
                Console.Write("*");
            }
        }


        return password;
    }
}