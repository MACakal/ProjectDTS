// using Microsoft.AspNetCore.Mvc;

// namespace ProjectDTS.Controllers
// {
//     public class WebshopController : Controller
//     {
//         private readonly ProductService _productService;

//         public WebshopController(ProductService productService)
//         {
//             _productService = productService;
//         }

//         public IActionResult Index()
//         {
//             var producten = _productService.GetAllProducts();
//             return View(producten);
//         }
//     }
// }