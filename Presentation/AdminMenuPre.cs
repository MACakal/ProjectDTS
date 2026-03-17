
namespace ProjectDTS;

public class AdminMenuPres
{
    // DatabaseService db = new DatabaseService();
    private ProductService _productService;
    private AdminManagerPres _adminManagerPres;
    private ViewProductPres _viewProductPres;
    public AdminMenuPres(ProductService productService, ViewProductPres viewProductPres)
    {
        _productService = productService;
        _adminManagerPres = new AdminManagerPres();
        _viewProductPres = viewProductPres;
    }

    // public void ShowAdminMenu()
    // {
    //     Console.Clear();
    //     while (true)
    //     {

    //         Console.WriteLine("\nAdmin Menu");
    //         Console.WriteLine("1. View Products");
    //         Console.WriteLine("2. Add Product");
    //         Console.WriteLine("0. Exit");

    //         var choice = Console.ReadLine();

    //         if (choice == "0")
    //         {

    //             Console.Clear();
    //             break;
    //         }

    //         if (choice == "1")
    //         {
    //             Console.Clear();
    //             _viewProductPres.Viewproducts();
    //         }

    //         if (choice == "2")
    //         {
    //             Console.WriteLine("Add product...");
    //             var product = _adminManagerPres.CreateProduct();
    //             _productService.AddProduct(product);

    //             Console.ForegroundColor = ConsoleColor.Cyan;
    //             Console.WriteLine("Product added successfully.");

    //             Console.ForegroundColor = ConsoleColor.Blue;
    //             Console.WriteLine("Please enter any key...");
    //             Console.ResetColor();
    //             Console.ReadKey();
    //             Console.Clear();


    //         }
    //     }
    // }
    public void ShowAdminMenu()
    {
        while (true)
        {
            Console.WriteLine(" ====Admin Menu==== ");
            Console.WriteLine("1. View Products");
            Console.WriteLine("2. Add Product");
            Console.WriteLine("0. Back");
            //     string[] options =
            //     {
            //     "View Products",
            //     "Add Product",
            //     "Back"
            // };

            // int choice = Menu.ShowMenu("Admin Menu", options);
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.Clear();
                    _viewProductPres.Viewproducts();
                    break;

                case "2":
                    Console.Clear();

                    Console.WriteLine("Add product...");
                    var product = _adminManagerPres.CreateProduct();
                    _productService.AddProduct(product);

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Product added successfully.");

                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("Press any key...");
                    Console.ResetColor();

                    Console.ReadKey();
                    Console.Clear();
                    break;

                case "0":
                    Console.Clear();
                    return;
            }
        }
    }
}
