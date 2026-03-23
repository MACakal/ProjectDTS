using ProjectDTS;

public class SortingMenu
{

    private readonly ProductService _productServce;
    private readonly ViewProductPres _viewProductPres;

    public SortingMenu(ProductService productservice, ViewProductPres viewproductpres)
    {
        _productServce = productservice;
        _viewProductPres = viewproductpres;
    }

    public void Show()
    {
        while (true)
        {
            Console.WriteLine("1. Popularity");
            Console.WriteLine("2. Views");
            Console.WriteLine("0. Back");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    {
                        Console.Clear();
                        SortPopularity(false);
                        break;
                    }
                case "2":
                    {
                        Console.Clear();
                        break;
                    }
                case "0":
                    {
                        Console.Clear();
                        return;
                    }
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }        
    }

    public void SortPopularity(bool ascending)
    {
        var products = _productServce.GetProductsSortedByPopularity(ascending);
        _viewProductPres.Viewproducts(products);
    }

}