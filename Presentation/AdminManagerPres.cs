namespace ProjectDTS;

public class AdminManagerPres
{


    public Product CreateProduct()
    {
        Console.WriteLine("Enter product name:");
        var name = Console.ReadLine();

        Console.WriteLine("Enter description:");
        var description = Console.ReadLine();

        Console.WriteLine("Enter category:");
        var category = Console.ReadLine();

        Console.WriteLine("Enter price:");
        decimal price;

        while (!decimal.TryParse(Console.ReadLine(), out price))
        {
            Console.WriteLine("Invalid price. Enter a valid number:");
        }

        Console.WriteLine("Enter rarity:");
        var rarity = Console.ReadLine();

        return new Product
        {
            Name = name,
            Description = description!,
            Category = category!,
            Price = price,
            Rarity = rarity!
        };
    }


    // private ProductService _productService;

    // public AdminManagerPres(ProductService productService)
    // {
    //     _productService = productService;
    // }
    // public void AdminAddProduct()
    // {
    //     Console.WriteLine("Enter product name:");
    //     var name = Console.ReadLine();

    //     Console.WriteLine("Enter description:");
    //     var description = Console.ReadLine();

    //     Console.WriteLine("Enter category:");
    //     var category = Console.ReadLine();

    //     Console.WriteLine("Enter price:");
    //     decimal price = decimal.Parse(Console.ReadLine());

    //     Console.WriteLine("Enter rarity:");
    //     var rarity = Console.ReadLine();

    //     var product = new Product
    //     {
    //         Name = name,
    //         Description = description!,
    //         Category = category!,
    //         Price = price,
    //         Rarity = rarity!
    //     };

    //     productService.AddProduct(product);

    //     Console.WriteLine("Product added successfully.");

}

// }