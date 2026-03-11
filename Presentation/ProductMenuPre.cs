namespace ProjectDTS;

public class ProductMenuPres
{
    private ViewProductPres _viewProductPres;
    private FilterMenu _filterMenu;

    public ProductMenuPres(ViewProductPres viewProductPres, FilterMenu filterMenu)
    {
        _viewProductPres = viewProductPres;
        _filterMenu = filterMenu;
    }

    public void Show()
    {
        Console.CursorVisible = false;
        while (true)
        {
            string[] options =
            {
                "Filter products",
                "View all products",
                "Back"
            };

            int choice = Menu.ShowMenu("Product Menu", options);

            switch (choice)
            {
                case 0:
                    Console.CursorVisible = true;
                    _filterMenu.Show();
                    break;

                case 1:
                    Console.CursorVisible = true;
                    _viewProductPres.Viewproducts();
                    break;

                case 2:
                    return;
            }
        }
    }
}