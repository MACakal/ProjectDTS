namespace ProjectDTS;

using Microsoft.Extensions.Configuration;
public class PastOrders
{
    // private static BasketService _basketService = new BasketService(new DatabaseService());
    private static BasketService _basketService =
    new BasketService(
        new DatabaseService(),
        new OrderMongoService(
            new MongoDbContext(
                new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build()
            )
        )
    );

    public static void ShowPastOrders()
    {
        var (start, end) = AskForDateRange();
        Console.Clear();
        decimal total;

        List<OrderLine> items = _basketService.GetPastOrderLines(
            UserSession.CurrentUser.Id,
            start,
            end,
            out total
        );

        Console.WriteLine($"\nShowing orders from {start:dd-MM-yyyy} to {end:dd-MM-yyyy}\n");
        Console.WriteLine($"Amount spent: €{total:N2}");
        Console.WriteLine("--- 🛒 Past Orders ---");

        if (items.Count == 0)
        {
            Console.WriteLine("No orders have been placed in this period.");
        }

        // Display orders with index and order date
        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            Console.WriteLine($"{i + 1}. {item.Name,-20} | {item.Quantity}x | €{item.Subtotal:N2} | Ordered: {item.OrderDate:dd-MM-yyyy HH:mm}");
        }

        Console.WriteLine("\nOptions:");
        Console.WriteLine("[1] View as a bar chart");
        Console.WriteLine("[2] Choose time period");
        Console.WriteLine("[3] Cancel an order (within 24h)");
        Console.WriteLine("[0] Back");

        string input = Console.ReadLine();

        if (input == "1")
        {
            ShowBarChart(items);
        }
        else if (input == "2")
        {
            ShowPastOrders();
        }
        else if (input == "3")
        {
            CancelOrder(items);
        }
    }

    private static void CancelOrder(List<OrderLine> items)
    {
        Console.WriteLine("Enter the number of the order to cancel:");
        if (!int.TryParse(Console.ReadLine(), out int index) || index < 1 || index > items.Count)
        {
            Console.WriteLine("Invalid selection.");
            return;
        }

        var order = items[index - 1];
        var timeSinceOrder = DateTime.Now - order.OrderDate;

        if (timeSinceOrder.TotalHours > 24)
        {
            Console.WriteLine("You can only cancel orders within 24 hours.");
            return;
        }

        Console.WriteLine($"Are you sure you want to cancel '{order.Name}'? (y/n)");
        string confirm = Console.ReadLine()?.ToLower();

        if (confirm == "y")
        {
            _basketService.CancelOrder(order.Id);
            Console.WriteLine("Order canceled successfully!");
        }
        else
        {
            Console.WriteLine("Cancellation aborted.");
        }
    }

    public static void ShowBarChart(List<OrderLine> items)
    {
        Console.Clear();
        Console.WriteLine("--- 📊 Spending Chart ---\n");

        if (items.Count == 0)
        {
            Console.WriteLine("No data available.");
            Console.ReadKey();
            return;
        }

        var grouped = items
            .GroupBy(i => i.Name)
            .Select(g => new
            {
                Name = g.Key,
                Total = g.Sum(x => x.Subtotal)
            })
            .OrderByDescending(x => x.Total)
            .ToList();

        decimal max = grouped.Max(i => i.Total);

        foreach (var item in grouped)
        {
            int barLength = (int)(item.Total / max * 30);
            string bar = new string('█', barLength);

            Console.WriteLine($"{item.Name,-15} | {bar} €{item.Total:N2}");
        }

        Console.ReadKey();
    }

    public static (DateTime start, DateTime end) AskForDateRange()
    {
        Console.WriteLine("Enter start date (dd-MM-yyyy) or press ENTER for last month:");
        string startInput = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(startInput))
        {
            DateTime endDefault = DateTime.Now;
            DateTime startDefault = endDefault.AddMonths(-1);
            return (startDefault, endDefault);
        }

        DateTime start;
        while (!DateTime.TryParseExact(startInput, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out start))
        {
            Console.WriteLine("Invalid date, use format dd-MM-yyyy:");
            startInput = Console.ReadLine();
        }

        Console.WriteLine("Enter end date (dd-MM-yyyy):");
        string endInput = Console.ReadLine();

        DateTime end;
        while (!DateTime.TryParseExact(endInput, "dd-MM-yyyy", null, System.Globalization.DateTimeStyles.None, out end))
        {
            Console.WriteLine("Invalid date, use format dd-MM-yyyy:");
            endInput = Console.ReadLine();
        }

        return (start, end);
    }
}