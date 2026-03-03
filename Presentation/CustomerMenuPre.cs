
// using ProjectDTS.DataSources;
namespace ProjectDTS;
// namespace ProjectDTS.Presentation
// {

public class CustomerMenuPre
{
    private ProductService _productService;
    public CustomerMenuPre(ProductService productService)
    {
        _productService = productService;
    }
    public void CustomerShow()
    {
        while (true)
        {

            Console.WriteLine("\nCustomer Menu");
            Console.WriteLine("1. View Products");
            Console.WriteLine("2. Make an Order");
            Console.WriteLine("0. Exit");

            var choice = Console.ReadLine();
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
            if (choice == "0") break;
        }
    }
}
// }
