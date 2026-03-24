namespace ProjectDTS;

public class PastOrders
{
    private static BasketService _basketService = new BasketService(new DatabaseService());

    public static void ShowPastOrders()
    {
        Console.Clear();
        decimal total;
        List<string> items = _basketService.GetPastOrderLinesLastMonth(UserSession.CurrentUser.Id, out total);

        Console.WriteLine($"Amount spend last month: €{total}");
        Console.WriteLine("--- 🛒 Past Orders ---");

        if (items.Count == 0)
        {
            Console.WriteLine("No orders have been placed in the last month.");
        }

        foreach (string item in items)
        {
            Console.WriteLine(item);
        }

        Console.WriteLine("Options:");
        Console.WriteLine("[1] View as a staff diagram (WIP, Not Finished)");
        Console.WriteLine("[0] Back");

        string input = Console.ReadLine();

        if (input == "1")
        {
            // Diagram moet nog toegevoegd worden
        }

    }
}