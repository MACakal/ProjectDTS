namespace ProjectDTS;

public class RatingDetail
{
    public string RatingKey { get; set; }
    public int ProductId { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; }
    public string ProductName { get; set; }
    public int RatingValue { get; set; }
    public string ReviewText { get; set; }
    public DateTime CreatedAt { get; set; }
}