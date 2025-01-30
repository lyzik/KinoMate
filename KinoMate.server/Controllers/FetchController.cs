using KinoMate.server.Database;
using KinoMate.server.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace KinoMate.server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FetchController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _baseUrl;
        private readonly ApplicationDbContext _context;


        public FetchController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ApplicationDbContext context)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiKey = configuration["TheMovieDb:ApiKey"];
            _baseUrl = configuration["TheMovieDb:BaseUrl"];
            _context = context;
        }


        [HttpGet("movie/{id}")]
        public async Task<IActionResult> GetMovieDetails(int id)
        {
            var url = $"{_baseUrl}/movie/{id}?api_key={_apiKey}&language=en-US";
            var videosUrl = $"{_baseUrl}/movie/{id}/videos?api_key={_apiKey}&language=en-US";
                
            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var movieDetails = JsonSerializer.Deserialize<MovieDetailsResponse>(jsonResponse);

                if (movieDetails == null)
                {
                    return NotFound($"No details found for movie with ID {id}.");
                }
                var videosResponse = await _httpClient.GetAsync(videosUrl);
                videosResponse.EnsureSuccessStatusCode();
                var videosJsonResponse = await videosResponse.Content.ReadAsStringAsync();
                var videosData = JsonSerializer.Deserialize<VideosResponse>(videosJsonResponse);

                var trailerLinks = videosData?.Results
                    .Where(video => video.Type == "Trailer" && video.Site == "YouTube")
                    .Select(video => $"https://www.youtube.com/watch?v={video.Key}")
                    .ToList();

                movieDetails.TrailerLinks = trailerLinks;
                var comments = _context.Comments
                    .Where(c => c.MovieId == id)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToList();

                var commentsResponse = new List<CommentsResponse>();
                foreach (var comment in comments)
                {
                    var username = _context.Users
                        .Where(u => u.Id == comment.UserId)
                        .Select(u => u.Username)
                        .FirstOrDefault();

                    commentsResponse.Add(new CommentsResponse
                    {
                        Id = comment.Id,
                        MovieId = comment.MovieId,
                        CommentText = comment.CommentText,
                        Username = username,
                        CreatedAt = comment.CreatedAt,
                        Rate = comment.Rate
                    });
                }

                movieDetails.Comments = commentsResponse;
                return Ok(movieDetails);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Error fetching movie details from TheMovieDB: {ex.Message}");
            }
        }
        [HttpGet("series/{id}")]
        public async Task<IActionResult> GetSeriesDetails(int id)
        {
            var url = $"{_baseUrl}/tv/{id}?api_key={_apiKey}&language=en-US";
            var videosUrl = $"{_baseUrl}/tv/{id}/videos?api_key={_apiKey}&language=en-US";

            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var seriesDetails = JsonSerializer.Deserialize<SeriesDetailsResponse>(jsonResponse);

                if (seriesDetails == null)
                {
                    return NotFound($"No details found for series with ID {id}.");
                }

                var videosResponse = await _httpClient.GetAsync(videosUrl);
                videosResponse.EnsureSuccessStatusCode();
                var videosJsonResponse = await videosResponse.Content.ReadAsStringAsync();
                var videosData = JsonSerializer.Deserialize<VideosResponse>(videosJsonResponse);

                var trailerLinks = videosData?.Results
                    .Where(video => video.Type == "Trailer" && video.Site == "YouTube")
                    .Select(video => $"https://www.youtube.com/watch?v={video.Key}")
                    .ToList();

                seriesDetails.TrailerLinks = trailerLinks;
                var comments = _context.Comments
                    .Where(c => c.MovieId == id)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToList();

                var commentsResponse = new List<CommentsResponse>();
                foreach (var comment in comments)
                {
                    var username = _context.Users
                        .Where(u => u.Id == comment.UserId)
                        .Select(u => u.Username)
                        .FirstOrDefault();

                    commentsResponse.Add(new CommentsResponse
                    {
                        Id = comment.Id,
                        MovieId = comment.MovieId,
                        CommentText = comment.CommentText,
                        Username = username,
                        CreatedAt = comment.CreatedAt,
                        Rate = comment.Rate
                    });
                }

                seriesDetails.Comments = commentsResponse;
                return Ok(seriesDetails);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Error fetching series details from TheMovieDB: {ex.Message}");
            }
        }

        [HttpGet("popularMovies")]
        public async Task<IActionResult> GetPopularMovies()
        {
            var url = $"{_baseUrl}/movie/popular?api_key={_apiKey}&language=en-US&page=1";

            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var moviesData = JsonSerializer.Deserialize<PopularMoviesResponse>(jsonResponse);

                if (moviesData == null || moviesData.Results == null)
                {
                    return NotFound("No movies found.");
                }

                var movies = moviesData.Results;

                return Ok(movies);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Error fetching data from TheMovieDB: {ex.Message}");
            }
        }

        [HttpGet("topSeries")]
        public async Task<IActionResult> GetTopSeries()
        {
            var url = $"{_baseUrl}/tv/popular?api_key={_apiKey}&language=en-US&page=1";

            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var seriesData = JsonSerializer.Deserialize<TopSeriesResponse>(jsonResponse);

                if (seriesData == null || seriesData.Results == null)
                {
                    return NotFound("No series found.");
                }

                var series = seriesData.Results;

                return Ok(series);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Error fetching data from TheMovieDB: {ex.Message}");
            }
        }
        [HttpGet("genres")]
        public async Task<IActionResult> GetGenres()
        {
            var url = $"{_baseUrl}/genre/movie/list?api_key={_apiKey}&language=en-US";

            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var genresData = JsonSerializer.Deserialize<GenresResponse>(jsonResponse);

                if (genresData == null || genresData.Genres == null)
                {
                    return NotFound("No genres found.");
                }

                var genres = genresData.Genres;

                return Ok(genres);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Error fetching genres from TheMovieDB: {ex.Message}");
            }
        }
        [HttpGet("seriesGenres")]
        public async Task<IActionResult> GetSeriesGenres()
        {
            var url = $"{_baseUrl}/genre/tv/list?api_key={_apiKey}&language=en-US";

            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var genresData = JsonSerializer.Deserialize<GenresResponse>(jsonResponse);

                if (genresData == null || genresData.Genres == null)
                {
                    return NotFound("No series genres found.");
                }

                var genres = genresData.Genres;

                return Ok(genres);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Error fetching series genres from TheMovieDB: {ex.Message}");
            }
        }
    }
}
