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
        Console.Clear();
        Console.WriteLine("\n--- Filter Menu ---");
        Console.WriteLine("1. Category");
        Console.WriteLine("2. Price");
        Console.WriteLine("3. Search by Name");
        Console.WriteLine("0. Back");

        var choice = Console.ReadLine();
        if (choice == "1")
        {
            Console.Clear();
            CategoryFilter();
            Console.WriteLine("\nPress any key to return to the filter menu.");
            Console.ReadLine();
        }
        else if (choice == "2")
        {
            Console.Clear();
            PriceFilter();
            Console.WriteLine("\nPress any key to return to the filter menu.");
            Console.ReadLine();
        }
        else if (choice == "3")
        {
            Console.Clear();
            SearchByName();
            Console.WriteLine("\nPress any key to return to the filter menu.");
            Console.ReadLine();
        }
        if (choice == "0") return;
    }

    public void CategoryFilter()
    {
        Console.WriteLine("What category do you want to filter by?");
        var cat = Console.ReadLine() ?? "";
        var products = _productService.GetProductsByCategory(cat);
        
        Console.WriteLine($"\nResults for '{cat}':");
        foreach (var p in products)
        {
            Console.WriteLine($"{p.Id} - {p.Name} - €{p.Price}");
        }
    }

    public void PriceFilter()
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
    public void SearchByName()
    {
        Console.WriteLine("\nEnter the name of the product to search for:");
        var input = Console.ReadLine() ?? "";

        if (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine("Search cannot be empty.");
            return;
        }

        var results = _productService.SearchProductsByName(input);

        Console.WriteLine($"\n---- Search results for '{input}' ----");
        if (results.Count == 0)
        {
            Console.WriteLine("No products found matching that name.");
        }
        else
        {
            foreach (var p in results)
            {
                Console.WriteLine($"{p.Id} - {p.Name} - €{p.Price}");
            }
        }
    }   
}