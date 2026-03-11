namespace ProjectDTS;

public class AdminManagerPres
{


    public Product CreateProduct()
    {
        Console.Clear();
        Console.WriteLine("Enter product name:");
        var name = Console.ReadLine();

        Console.WriteLine("Enter description:");
        var description = Console.ReadLine();

        Console.WriteLine("Enter category:");
        var category = Console.ReadLine();

        Console.WriteLine("Enter price:");
        decimal price;

        while (!decimal.TryParse(Console.ReadLine(), out price) | price <= 0)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Invalid price. Enter a valid number:");
            Console.ResetColor();

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
}

