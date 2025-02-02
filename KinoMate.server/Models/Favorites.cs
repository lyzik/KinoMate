namespace KinoMate.server.Models
{
    public class Favorites
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public List<int> MoviesId { get; set; }
        public List<int> SeriesId { get; set; }
        public List<int> MoviesNotificationId { get; set; }
        public List<int> SeriesNotificationId { get; set; }

    }
}
