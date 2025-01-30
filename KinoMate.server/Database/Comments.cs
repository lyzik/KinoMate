namespace KinoMate.server.Database
{
    public class Comment
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public string CommentText { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal Rate { get; set; }
        public CommentMediaType? MediaType { get; set; }
    }

    public enum CommentMediaType
    {
        Movie,
        Series
    }

}