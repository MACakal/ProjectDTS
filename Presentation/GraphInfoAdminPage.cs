namespace ProjectDTS;

public class GraphInfoAdminPage
{
    private readonly GraphInfoLogic _logic;

    public GraphInfoAdminPage(Graphservice graphservice)
    {
        _logic = new GraphInfoLogic(graphservice);
    }

    public void Show()
    {
        while (true)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔══════════════════════════════════════╗");
            Console.WriteLine("║     Graph Database — Analytics       ║");
            Console.WriteLine("╚══════════════════════════════════════╝");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("[1] Producten vaak samen gekocht");
            Console.WriteLine("[2] Klanten met vergelijkbaar koopgedrag");
            Console.WriteLine("[3] Klanten per categorie");
            Console.WriteLine("[4] Producten beste / slechtste reviews");
            Console.WriteLine("[5] Meest gekochte producten");
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("[0] Terug");
            Console.ResetColor();
            Console.WriteLine();
            Console.Write("Kies query: ");

            switch (Console.ReadLine())
            {
                case "1": ShowProductsBoughtTogether(); break;
                case "2": ShowSimilarCustomers(); break;
                case "3": ShowCategoryRetention(); break;
                case "4": ShowProductRatings(); break;
                case "5": ShowMostPurchasedProducts(); break;
                case "0": return;
            }
        }
    }

    private void ShowProductsBoughtTogether()
    {
        Console.Clear();
        Console.WriteLine("=== Producten vaak samen gekocht ===");
        Console.WriteLine("Vraag: Welke producten komen het vaakst samen voor in een bestelling?");
        Console.WriteLine("Relaties: Order -> Product (twee producten per order)");
        Console.WriteLine();

        var results = _logic.GetProductsBoughtTogether();

        if (results.Count == 0)
        {
            Console.WriteLine("Geen data beschikbaar.");
            Console.ReadKey();
            return;
        }

        Console.WriteLine($"  {"Product 1",-30} {"Product 2",-30}");
        Console.WriteLine(new string('-', 65));

        foreach (var (product1, product2) in results)
            Console.WriteLine($"  {product1,-30} {product2,-30}");

        Console.WriteLine();
        Console.WriteLine("Uitleg: Deze producten worden vaak samen gekocht, geschikt voor aanbevelingen.");
        Console.WriteLine("\nDruk op een toets...");
        Console.ReadKey();
    }

    private void ShowSimilarCustomers()
    {
        Console.Clear();
        Console.WriteLine("=== Klanten met vergelijkbaar koopgedrag ===");
        Console.WriteLine("Vraag: Welke klanten kochten dezelfde producten?");
        Console.WriteLine("Relaties: Customer -> Order -> Product <- Order <- Customer");
        Console.WriteLine();

        var results = _logic.GetSimilarCustomers();

        if (results.Count == 0)
        {
            Console.WriteLine("Geen data beschikbaar.");
            Console.ReadKey();
            return;
        }

        Console.WriteLine($"  {"Klant 1",-25} {"Klant 2",-25} {"Gedeeld product"}");
        Console.WriteLine(new string('-', 75));

        foreach (var (customer1, customer2, sharedProduct) in results)
            Console.WriteLine($"  {customer1,-25} {customer2,-25} {sharedProduct}");

        Console.WriteLine();
        Console.WriteLine("Uitleg: Klanten die dezelfde producten kopen zijn ideaal voor gerichte marketing.");
        Console.WriteLine("\nDruk op een toets...");
        Console.ReadKey();
    }

    private void ShowCategoryRetention()
    {
        Console.Clear();
        Console.WriteLine("=== Klanten per categorie ===");
        Console.WriteLine("Vraag: Welke klanten kopen in welke categorie?");
        Console.WriteLine("Relaties: Customer -> Order -> Product -> Category");
        Console.WriteLine();

        var results = _logic.GetCategoryRetention();

        if (results.Count == 0)
        {
            Console.WriteLine("Geen data beschikbaar.");
            Console.ReadKey();
            return;
        }

        Console.WriteLine($"  {"Klant",-25} {"Categorie",-20} {"Product"}");
        Console.WriteLine(new string('-', 75));

        foreach (var (customer, category, product) in results)
            Console.WriteLine($"  {customer,-25} {category,-20} {product}");

        Console.WriteLine();
        Console.WriteLine("Uitleg: Overzicht van welke klanten in welke categorieën kopen.");
        Console.WriteLine("\nDruk op een toets...");
        Console.ReadKey();
    }

    private void ShowProductRatings()
    {
        Console.Clear();
        Console.WriteLine("=== Beste en slechtste productreviews ===");
        Console.WriteLine("Vraag: Welke producten scoren het hoogst en laagst?");
        Console.WriteLine("Relaties: Review -> Product");
        Console.WriteLine("Aggregatie: AVG(rating), COUNT(reviews), ORDER BY DESC");
        Console.WriteLine();

        var results = _logic.GetProductRatings();

        if (results.Count == 0)
        {
            Console.WriteLine("Geen data beschikbaar.");
            Console.ReadKey();
            return;
        }

        double max = results.Max(r => r.AvgRating);

        foreach (var (product, avgRating, reviewCount) in results)
        {
            int barLength = (int)(avgRating / max * 30);
            string bar = new string('█', barLength);

            Console.ForegroundColor = avgRating >= 4.0 ? ConsoleColor.Green : avgRating <= 2.5 ? ConsoleColor.Red : ConsoleColor.White;
            Console.WriteLine($"{product,-30} | {bar} {avgRating:F1} ({reviewCount} reviews)");
            Console.ResetColor();
        }

        Console.WriteLine();
        Console.WriteLine("Uitleg: Groen = goed (>= 4.0), Rood = slecht (<= 2.5).");
        Console.WriteLine("\nDruk op een toets...");
        Console.ReadKey();
    }

    private void ShowMostPurchasedProducts()
    {
        Console.Clear();
        Console.WriteLine("=== Meest gekochte producten ===");
        Console.WriteLine("Vraag: Welke producten zijn het vaakst gekocht?");
        Console.WriteLine("Relaties: Order -> Product -> Category");
        Console.WriteLine();

        var results = _logic.GetMostPurchasedProducts();

        if (results.Count == 0)
        {
            Console.WriteLine("Geen data beschikbaar.");
            Console.ReadKey();
            return;
        }

        int max = results.Max(r => r.TimesPurchased);

        foreach (var (product, category, timesPurchased) in results)
        {
            int barLength = (int)((double)timesPurchased / max * 30);
            string bar = new string('█', barLength);

            Console.WriteLine($"{product,-30} | {bar} {timesPurchased} ({category})");
        }

        Console.WriteLine();
        Console.WriteLine("Uitleg: Populaire producten die goed verkopen.");
        Console.WriteLine("\nDruk op een toets...");
        Console.ReadKey();
    }
}