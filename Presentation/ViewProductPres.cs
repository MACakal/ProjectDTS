namespace ProjectDTS;

public class ViewProductPres
{
    private ProductService _productService;

    public ViewProductPres(ProductService productService)
    {
        _productService = productService;

    }
    public void Viewproducts()
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

            Console.WriteLine($"{"ID",-5} {"Name",-20} {"Price",10}");
            Console.WriteLine(new string('-', 40));
            foreach (var p in products)
            {

                Console.WriteLine($"{p.Id,-5} {p.Name,-20} {p.Price,10}€");
            }
        }
        Console.WriteLine("Press any key to return...");
        Console.ReadKey();
        Console.Clear();
    }

}