namespace KinoMate.server.Models
{
    public class CommentsResponse
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public string CommentText { get; set; }
        public string Username { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal Rate { get; set; }
    }
}
