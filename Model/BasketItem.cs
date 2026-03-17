public class BasketItem
{
    public int ProductId { get; set; }     // p.id
    public string Name { get; set; }       // p.name
    public int Quantity { get; set; }      // oi.quantity
    public decimal Price { get; set; }     // p.price

    public override string ToString()
    {
        return $"{Quantity} x {Name} at {Price}€ each (Subtotal: {Subtotal}€)";
    }
    public decimal Subtotal => Quantity * Price;
}