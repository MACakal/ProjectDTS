namespace ProjectDTS;


public class CustomerMenuPre
{

    private ViewProductPres _viewProductPres;
    private FilterMenu _filterMenu;
    private AccountPre _AccountPre;
    private ProductLogic productLogic = new ProductLogic();
    public CustomerMenuPre(ViewProductPres viewProductPres, FilterMenu filterMenu, AccountPre accountPre) //ProductService productService, 
    {
        _viewProductPres = viewProductPres;
        _filterMenu = filterMenu;
        _AccountPre = accountPre;
    }

    public void CustomerShow(User user)
    {
        while (true)
        {
            Console.Clear();
            //BreadcrumbManager.Render();

            Console.WriteLine("\nCustomer Menu");
            Console.WriteLine("1. View Products");
            Console.WriteLine("2. Make an Order");
            Console.WriteLine("3. View past Orders");
            Console.WriteLine("4. Filter Products");
            Console.WriteLine("5. View products ranking");
            Console.WriteLine("6. View account information");
            Console.WriteLine("7. View recently viewed products");
            Console.WriteLine("0. Exit");

            var choice = Console.ReadLine();

            if (choice == "1")
            {
                //BreadcrumbManager.Push("View Products");
                Console.Clear();
                //BreadcrumbManager.Render();
                _viewProductPres.Viewproducts();
                Console.WriteLine("\nPress any key to return to the customer menu.");
                Console.ReadLine();
            }
            if (choice == "2")
            {
                //BreadcrumbManager.Push("Order");
                Console.Clear();
                //BreadcrumbManager.Render();
                if (UserSession.CurrentUser == null)
                {
                    Console.WriteLine("⚠️ You must be logged in to make an order!");
                    Console.WriteLine("\nPress any key to return to the customer menu.");
                    Console.ReadLine();
                }
                else
                {

                    BasketMenu.ShowBasket();
                }
            }
            if (choice == "3")
            {
                //BreadcrumbManager.Push("Past Orders");
                Console.Clear();
                //BreadcrumbManager.Render();
                if (UserSession.CurrentUser == null)
                {
                    Console.WriteLine("⚠️ You must be logged in to view past order!");
                    Console.WriteLine("\nPress any key to return to the customer menu.");
                    Console.ReadLine();
                }
                else
                {
                    PastOrders.ShowPastOrders();
                    Console.WriteLine("\nPress any key to return to the customer menu.");
                    Console.ReadLine();
                }
            }
            if (choice == "4")
            {
                //BreadcrumbManager.Push("Filter Products");
                Console.Clear();
                //BreadcrumbManager.Render();
                _filterMenu.Show();

                Console.WriteLine("\nPress any key to return to the customer menu.");
                Console.ReadLine();
            }
            if (choice == "5")
            {
                //BreadcrumbManager.Push("Products Ranking");
                Console.Clear();
                //BreadcrumbManager.Render();
                ProductsRanking.Start();

                Console.WriteLine("\nPress any key to return to the customer menu.");
                Console.ReadLine();
            }
            if (choice == "6")
            {
                //BreadcrumbManager.Push("Account Information");
                Console.Clear();
                //BreadcrumbManager.Render();
                _AccountPre.AccountInformation(user);

            }
            if (UserSession.CurrentUser == null)
            {
                return;
            }
            if (choice == "7")
            {
                Console.Clear();

                var products = productLogic.GetRecentProducts(user.Id).Result;

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("===== Recently Viewed Products =====\n");
                Console.ResetColor();

                if (products.Count == 0)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine("No recently viewed products.");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"{"ID",-5} {"Name",-30} {"Price",10}");
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine(new string('-', 50));

                    foreach (var p in products)
                    {
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.Write($"{p.Id,-5}");

                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write($"{p.Name,-30}");

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"€{p.Price,10}");

                        Console.ResetColor();
                    }


                    // Console.WriteLine("\n1. View product");
                    // Console.WriteLine("2. Add to basket");
                    // Console.WriteLine("0. Back");

                    // var action = Console.ReadLine();

                    // switch (action)
                    // {
                    //     case "1":
                    //         BasketMenu.ViewItem();
                    //         break;
                    //     case "2":

                    //         Console.Write("Quantity: ");

                    //         int qty = int.Parse(Console.ReadLine());
                    //         BasketMenu.AddToBasketDirect(selectedId, qty);
                    //         break;
                    // }
                }

                Console.WriteLine("\nPress any key to return...");
                Console.ReadKey();
            }

            if (choice == "0") break;
        }
    }

}




