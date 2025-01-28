namespace KinoMate.server.Database
{
    public class Comment
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public string CommentText { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Rate { get; set; }
    }
}