namespace ProjectDTS;

public class Rating
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int UserId { get; set; }
    public int RatingValue { get; set; } // 1-5 stars
    public string ReviewText { get; set; }
    public DateTime CreatedAt { get; set; }
}
