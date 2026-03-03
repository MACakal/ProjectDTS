
namespace ProjectDTS;

public class AdminMenuPres
{
    // DatabaseService db = new DatabaseService();
    private ProductService _productService;
    private AdminManagerPres _adminManagerPres;
    public AdminMenuPres(ProductService productService)
    {
        _productService = productService;
        _adminManagerPres = new AdminManagerPres();
    }

    public void ShowAdminMenu()
    {
        while (true)
        {

            Console.WriteLine("\nAdmin Menu");
            Console.WriteLine("1. View Products");
            Console.WriteLine("2. Add Product");
            Console.WriteLine("0. Exit");

            var choice = Console.ReadLine();

            if (choice == "0") break;

            if (choice == "1")
            {
                Console.Clear();
                var products = _productService.GetAllProducts();

                Console.WriteLine("\n---- Product List ----");
                if (products.Count == 0)
                {
                    Console.Clear();
                    Console.WriteLine("There are no products to show.");

                }
                else
                {

                    foreach (var p in products)
                    {
                        Console.WriteLine($"{p.Id} - {p.Name} - {p.Price}€");
                    }
                }
            }

            if (choice == "2")
            {
                Console.WriteLine("Add product...");
                var product = _adminManagerPres.CreateProduct();
                _productService.AddProduct(product);
                Console.WriteLine("Product added successfully.");


                // Console.WriteLine("Product added successfully.");

            }
        }
    }
}
