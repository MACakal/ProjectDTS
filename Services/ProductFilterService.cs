namespace ProjectDTS;

public class ProductFilterService
{
    private readonly ProductService _productService;

    // Constructor: we geven de ProductService door
    public ProductFilterService(ProductService productService)
    {
        _productService = productService;
    }

    public void FilterByCategory()
    {
        Console.Write("Enter category: ");
        var category = Console.ReadLine() ?? "";
        
        var results = _productService.GetProductsByCategory(category);
        DisplayResults(results, $"Category: {category}");
    }

    // Handige kleine methode om herhaling van code te voorkomen
    private void DisplayResults(List<Product> products, string filterName)
    {
        Console.WriteLine($"\n--- Results for {filterName} ---");
        if (products.Count == 0) Console.WriteLine("No products found.");
        else
        {
            foreach (var p in products)
                Console.WriteLine($"{p.Id} - {p.Name} - {p.Price}€");
        }
    }
}