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

        Console.WriteLine("Choose category:");
        var category = ChooseCategory();

        Console.WriteLine("Enter price:");
        decimal price;

        while (!decimal.TryParse(Console.ReadLine(), out price) || price <= 0)
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

    public Product? EditProduct()
    {
        var db = new DatabaseService();
        var productService = new ProductService(db);

        Console.Clear();

        var products = productService.GetAllProducts();

        Console.WriteLine("\n---- Product List ----");

        if (products.Count == 0)
        {
            Console.WriteLine("There are no products to show.");
            return null;
        }

        Console.WriteLine($"{"ID",-5} {"Name",-20} {"Category",-20} {"Price",10}");
        Console.WriteLine(new string('-', 60));

        foreach (var p in products)
        {
            Console.WriteLine($"{p.Id,-5} {p.Name,-20} {p.Category,-20} {p.Price,10}€");
        }

        Console.WriteLine("\nEnter product id:");
        int id = int.Parse(Console.ReadLine());

        Product product = productService.GetById(id);

        bool editing = true;

        while (editing)
        {
            Console.Clear();

            Console.WriteLine("Editing product:");
            Console.WriteLine($"Name: {product.Name}");
            Console.WriteLine($"Description: {product.Description}");
            Console.WriteLine($"Category: {product.Category}");
            Console.WriteLine($"Price: {product.Price}");
            Console.WriteLine($"Rarity: {product.Rarity}");

            Console.WriteLine();
            Console.WriteLine("1 Change name");
            Console.WriteLine("2 Change description");
            Console.WriteLine("3 Change category");
            Console.WriteLine("4 Change price");
            Console.WriteLine("5 Change rarity");
            Console.WriteLine("6 Save and exit");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.WriteLine("Enter new name:");
                    product.Name = Console.ReadLine();
                    break;

                case "2":
                    Console.WriteLine("Enter new description:");
                    product.Description = Console.ReadLine();
                    break;

                case "3":
                    product.Category = ChooseCategory();
                    break;

                case "4":
                    Console.WriteLine("Enter new price:");

                    decimal price;
                    while (!decimal.TryParse(Console.ReadLine(), out price) || price <= 0)
                    {
                        Console.WriteLine("Invalid price. Enter a valid number:");
                    }

                    product.Price = price;
                    break;

                case "5":
                    Console.WriteLine("Enter new rarity:");
                    product.Rarity = Console.ReadLine();
                    break;

                case "6":
                    editing = false;
                    break;
            }
        }

        productService.UpdateProduct(product);

        Console.WriteLine("Product updated successfully!");

        return product;
    }


    public static string ChooseCategory()
    {
        string[] categories =
        {
         "Electronics",
        "Books",
        "Games",
        "Toys",
        "Home & Kitchen",
        "Clothing",
        "Sports",
        "Beauty",
        "Office",
        "Pet Supplies"
        };

        Console.WriteLine("Choose a category:");

        for (int i = 0; i < categories.Length; i++)
        {
            Console.WriteLine($"{i + 1}. {categories[i]}");
        }

        int choice = int.Parse(Console.ReadLine());

        return categories[choice - 1];
    }

}

