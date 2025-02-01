namespace KinoMate.server.Database
{
    public class ForumPost
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class PostLike
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; }
    }
    public class PostComments
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ForumPostRequest
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }
    public class PostLikeRequest
    {
        public int PostId { get; set; }
    }
    public class PostCommentsRequest
    {
        public int PostId { get; set; }
        public string Description { get; set; }
    }
}
