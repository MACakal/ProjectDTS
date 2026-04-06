public class OrderLine
{
    public int Id { get; set; }      
    public string Name { get; set; }
    public int Quantity { get; set; }
    public decimal Subtotal { get; set; }
    public DateTime OrderDate { get; set; } 
}