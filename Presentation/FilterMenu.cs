using ProjectDTS;

namespace ProjectDTS;

public class FilterMenu
{
    private readonly ProductService _productService;
    

    public FilterMenu(ProductService productService)
    {
        _productService = productService;

    }

    public void Show()
    {
        Console.WriteLine("\n--- Filter Menu ---");
        Console.WriteLine("1. Category");
        Console.WriteLine("2. Price");
        Console.WriteLine("0. Back");

        var choice = Console.ReadLine();
        if (choice == "1")
        {
            Console.WriteLine("What category?");
            var cat = Console.ReadLine() ?? "";
            var products = _productService.GetProductsByCategory(cat);
            
            Console.WriteLine($"\nResults for '{cat}':");
            foreach (var p in products)
            {
                Console.WriteLine($"{p.Id} - {p.Name} - €{p.Price}");
            }
        }
        else if (choice == "2")
        {
            Console.WriteLine("\nHow do you want to sort?");
            Console.WriteLine("1. Cheapest first (Low to High)");
            Console.WriteLine("2. Most expensive first (High to Low)");
            
            var sortChoice = Console.ReadLine();
            bool ascending = (sortChoice == "1");

            var results = _productService.GetProductsSortedByPrice(ascending);

            Console.WriteLine("\n---- Sorted Products ----");
            foreach (var p in results)
            {
                Console.WriteLine($"{p.Id} - {p.Name} - €{p.Price}");
            }
        }
    }
}