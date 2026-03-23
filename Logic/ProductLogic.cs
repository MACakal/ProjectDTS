using ProjectDTS;

public class ProductLogic
{
    private static ProductService _productService = new ProductService(new DatabaseService());

    public List<Product> GetTop3ChetProducts()
    {
        return _productService.GetTop3ChetProducts();
    }

    public List<Product> GetTop3ExpProducts()
    {
        return _productService.GetTop3ExpProducts();
    }
}