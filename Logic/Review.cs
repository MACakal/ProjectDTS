public class Review
{
    public string ID { get; set; }
    public string ProductID { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; }
    public DateTime Date { get; set; }

    public override string ToString()
    {
        return $"[{ProductID}] {Rating}/5 - {Comment} ({Date:dd/MM/yyyy})";
    }
}