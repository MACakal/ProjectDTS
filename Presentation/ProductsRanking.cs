using ProjectDTS;

static class ProductsRanking
{
    static private ProductLogic productLogic = new();

    public static void Start()
    {
        Console.Clear();
        Console.WriteLine("Welcome to the Product ranking page");
        Console.WriteLine("[1] See top 3 cheapest products");
        Console.WriteLine("[2] See top 3 most expensive products");
        Console.WriteLine("[3] Go back to user menu");

        string opt = Console.ReadLine();

        if (opt == "1")
        {
            Console.Clear();
            ViewCheap();
        }
        else if (opt == "2")
        {
            Console.Clear();
            ViewExp();
        }
        else if (opt == "3")
        {
            Console.Clear();
            return;
        }
        else
        {
            Start();
        }
    }
    public static void ViewCheap()
    {
        List<Product> feedlist = productLogic.GetTop3ChetProducts();

        if (feedlist.Count == 0)
        {
            Console.WriteLine("No Products in Database");
        }
        else
        {
            Console.WriteLine("=== Top 3 Cheapest Products ===\n");

            int maxPrice = 0;
            foreach (var p in feedlist)
            {
                if ((int)p.Price > maxPrice)
                    maxPrice = (int)p.Price;
            }

            foreach (var product in feedlist)
            {
                int length = ((int)product.Price * 20) / maxPrice;
                int i = 0;

                Console.Write(product.Name.PadRight(15) + " | ");

                while (i < length)
                {
                    Console.Write("#");
                    i++;
                }

                Console.WriteLine(" €" + product.Price);
            }
        }

        Console.WriteLine("\nPress ENTER to continue");
        Console.ReadLine();
        Console.Clear();
        Start();
    }

    public static void ViewExp()
    {
        List<Product> feedlist = productLogic.GetTop3ExpProducts();

        if (feedlist.Count == 0)
        {
            Console.WriteLine("No Products in Database");
        }
        else
        {
            Console.WriteLine("=== Top 3 Most Expensive Products ===\n");

            int maxPrice = 0;
            foreach (var p in feedlist)
            {
                if ((int)p.Price > maxPrice)
                    maxPrice = (int)p.Price;
            }

            foreach (var product in feedlist)
            {
                int length = ((int)product.Price * 20) / maxPrice;
                int i = 0;

                Console.Write(product.Name.PadRight(15) + " | ");

                while (i < length)
                {
                    Console.Write("#");
                    i++;
                }

                Console.WriteLine(" €" + product.Price);
            }
        }

        Console.WriteLine("\nPress ENTER to continue");
        Console.ReadLine();
        Console.Clear();
        Start();
    }
}