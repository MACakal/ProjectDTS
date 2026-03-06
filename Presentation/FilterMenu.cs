public static class FilterMenu
{
    public static void FilterProducts()
    {
        Console.WriteLine("what do you want to filter by?");

        Console.WriteLine("1. category");
        Console.WriteLine("2. price");
        Console.WriteLine("3. rarity");
        Console.WriteLine("0. back");
        var choice = Console.ReadLine();
        switch(choice)
        {
            case "1":
                Console.WriteLine("what category do you want to filter by?");
                var category = Console.ReadLine();
                break;
            case "2":
                Console.WriteLine("what price do you want to filter by?");
                Console.WriteLine("1. lower than");
                Console.WriteLine("2. higher than");
                Console.WriteLine("3. equal to");
                Console.WriteLine("4. from high to low prices");
                Console.WriteLine("5. from low to high prices");
                var price = Console.ReadLine();
                // filter by price
                break;
            case "3":
                Console.WriteLine("what rarity do you want to filter by?");
                var rarity = Console.ReadLine();
                // filter by rarity
                break;
            case "0":
                // go back
                break;
        }
    }
}