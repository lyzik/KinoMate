using KinoMate.server.Database;
using KinoMate.server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using static KinoMate.server.Models.SearchSeries;

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
            var providersUrl = $"{_baseUrl}/movie/{id}/watch/providers?api_key={_apiKey}";

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

                movieDetails.TrailerLinks = videosData?.Results
                    .Where(video => video.Type == "Trailer" && video.Site == "YouTube")
                    .Select(video => $"https://www.youtube.com/watch?v={video.Key}")
                    .ToList();

                var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

                if (user != null)
                {
                    var favorites = await _context.Favorites.FirstOrDefaultAsync(f => f.UserId == user.Id);
                    movieDetails.IsFavorite = favorites?.MoviesId.Contains(id) ?? false;
                    movieDetails.HasNotification = favorites?.MoviesNotificationId.Contains(id) ?? false;
                }

                var comments = _context.Comments
                    .Where(c => c.MovieId == id && c.MediaType == CommentMediaType.Movie)
                    .OrderByDescending(c => c.CreatedAt)
                    .ToList();

                var commentsResponse = comments.Select(comment => new CommentsResponse
                {
                    Id = comment.Id,
                    MovieId = comment.MovieId,
                    CommentText = comment.CommentText,
                    Username = _context.Users.Where(u => u.Id == comment.UserId)
                                             .Select(u => u.Username)
                                             .FirstOrDefault(),
                    CreatedAt = comment.CreatedAt,
                    Rate = comment.Rate
                }).ToList();

                movieDetails.Comments = commentsResponse;

                var providersResponse = await _httpClient.GetAsync(providersUrl);
                providersResponse.EnsureSuccessStatusCode();
                var providersJsonResponse = await providersResponse.Content.ReadAsStringAsync();
                var providersData = JsonSerializer.Deserialize<WatchProvidersResponse>(providersJsonResponse);

                if (providersData?.Results != null && providersData.Results.ContainsKey("US"))
                {
                    var providers = providersData.Results["US"];
                    movieDetails.StreamingLink = providers.Link;
                    movieDetails.StreamingPlatforms = providers.Flatrate?.Select(provider => new StreamingPlatform
                    {
                        Name = provider.ProviderName,
                        LogoUrl = $"https://image.tmdb.org/t/p/w500{provider.LogoPath}"
                    }).ToList();
                }

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
            var providersUrl = $"{_baseUrl}/tv/{id}/watch/providers?api_key={_apiKey}";

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
                    .Where(c => c.MovieId == id && c.MediaType == CommentMediaType.Series)
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


                var providersResponse = await _httpClient.GetAsync(providersUrl);
                providersResponse.EnsureSuccessStatusCode();
                var providersJsonResponse = await providersResponse.Content.ReadAsStringAsync();
                var providersData = JsonSerializer.Deserialize<WatchProvidersResponse>(providersJsonResponse);

                if (providersData?.Results != null && providersData.Results.ContainsKey("US"))
                {
                    var providers = providersData.Results["US"];
                    seriesDetails.StreamingLink = providers.Link;
                    seriesDetails.StreamingPlatforms = providers.Flatrate?.Select(provider => new StreamingPlatform
                    {
                        Name = provider.ProviderName,
                        LogoUrl = $"https://image.tmdb.org/t/p/w500{provider.LogoPath}"
                    }).ToList();
                }


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
        [HttpGet("search/{query}")]
        public async Task<IActionResult> SearchMoviesAndSeries(string query)
        {
            var movieUrl = $"{_baseUrl}/search/movie?api_key={_apiKey}&language=en-US&query={query}&page=1";
            var seriesUrl = $"{_baseUrl}/search/tv?api_key={_apiKey}&language=en-US&query={query}&page=1";

            try
            {
                var movieResponse = await _httpClient.GetAsync(movieUrl);
                movieResponse.EnsureSuccessStatusCode();
                var movieJsonResponse = await movieResponse.Content.ReadAsStringAsync();
                var movieData = JsonSerializer.Deserialize<SearchMoviesResponse>(movieJsonResponse);

                var seriesResponse = await _httpClient.GetAsync(seriesUrl);
                seriesResponse.EnsureSuccessStatusCode();
                var seriesJsonResponse = await seriesResponse.Content.ReadAsStringAsync();
                var seriesData = JsonSerializer.Deserialize<SearchSeriesResponseMapping>(seriesJsonResponse);

                var results = new List<SearchResult>();

                if (movieData?.Results != null)
                {
                    results.AddRange(movieData.Results.Select(m => new SearchResult
                    {
                        Id = m.Id,
                        Title = m.Title,
                        Type = "Movie"
                    }));
                }

                if (seriesData?.Results != null)
                {
                    results.AddRange(seriesData.Results.Select(s => new SearchResult
                    {
                        Id = s.Id,
                        Title = s.Name,
                        Type = "Series"
                    }));
                }

                return Ok(results);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Error fetching search results from TheMovieDB: {ex.Message}");
            }
        }
        [HttpGet("upcomingMovies/{year}/{month}")]
        public async Task<IActionResult> GetUpcomingMoviesByMonth(int year, int month)
        {
            var firstDay = new DateTime(year, month, 1).ToString("yyyy-MM-dd");
            var lastDay = new DateTime(year, month, DateTime.DaysInMonth(year, month)).ToString("yyyy-MM-dd");

            var url = $"{_baseUrl}/discover/movie?api_key={_apiKey}&language=en-US&region=US" +
                      $"&primary_release_date.gte={firstDay}&primary_release_date.lte={lastDay}" +
                      $"&sort_by=popularity_desc";

            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var jsonResponse = await response.Content.ReadAsStringAsync();
                var moviesData = JsonSerializer.Deserialize<UpcomingMoviesResponse>(jsonResponse);

                if (moviesData == null || moviesData.Results == null)
                {
                    return NotFound("No upcoming movies found for the specified month.");
                }

                return Ok(moviesData.Results);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Error fetching upcoming movie releases: {ex.Message}");
            }
        }
        [HttpGet("getFavoriteMovies")]
        [Authorize]
        public async Task<IActionResult> GetFavoriteMovies()
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized("User ID is missing.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var favorites = await _context.Favorites.FirstOrDefaultAsync(f => f.UserId == user.Id);
            if (favorites == null || favorites.MoviesId == null || !favorites.MoviesId.Any())
            {
                return Ok(new List<object>());
            }

            var movieDetails = new List<object>();

            foreach (var movieId in favorites.MoviesId)
            {
                var movieUrl = $"{_baseUrl}/movie/{movieId}?api_key={_apiKey}&language=en-US";

                try
                {
                    var response = await _httpClient.GetAsync(movieUrl);
                    response.EnsureSuccessStatusCode();
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var movieData = JsonSerializer.Deserialize<MovieDetailsResponse>(jsonResponse);

                    if (movieData != null)
                    {
                        movieDetails.Add(new
                        {
                            Id = movieData.Id,
                            Title = movieData.Title,
                            Poster = movieData.PosterPath
                        });
                    }
                }
                catch (HttpRequestException)
                {
                    continue;
                }
            }

            return Ok(movieDetails);
        }


    }
}
