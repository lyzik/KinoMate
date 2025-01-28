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

        public FetchController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiKey = configuration["TheMovieDb:ApiKey"];
            _baseUrl = configuration["TheMovieDb:BaseUrl"];
        }


        [HttpGet("movie/{id}")]
        public async Task<IActionResult> GetMovieDetails(int id)
        {
            var url = $"{_baseUrl}/movie/{id}?api_key={_apiKey}&language=en-US";

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

                return Ok(movieDetails);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, $"Error fetching movie details from TheMovieDB: {ex.Message}");
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
