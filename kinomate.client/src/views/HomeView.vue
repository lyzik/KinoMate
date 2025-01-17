<template>
  <v-container>
    <v-row class="mt-10">
      <v-col cols="12">
        <h2
          class="text-h4 font-weight-bold text-center mb-6"
          style="color: yellow"
        >
          Popular Movies
        </h2>
        <v-slide-group width="100%">
          <v-slide-item v-for="(movie, index) in movies" :key="index">
            <div class="movie-image-wrapper">
              <v-img
                :src="`https://image.tmdb.org/t/p/w500${movie.poster_path}`"
                :alt="movie.title"
                class="movie-image"
                style="border-radius: 16px; overflow: hidden"
                width="250"
                height="100%"
                cover
              ></v-img>
              <div class="movie-overlay">
                <h3 class="movie-title">{{ movie.title }}</h3>
                <p class="movie-release-date">
                  {{ movie.release_date }}
                </p>
                <p class="movie-genres">
                  {{ mapGenres(movie.genre_ids, movieGenres) }}
                </p>
              </div>
            </div>
          </v-slide-item>
        </v-slide-group>
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12">
        <h2
          class="text-h4 font-weight-bold text-center mb-6 mt-10"
          style="color: yellow"
        >
          Popular TV Series
        </h2>
        <v-slide-group>
          <v-slide-item v-for="(series, index) in series" :key="index">
            <div class="movie-image-wrapper">
              <v-img
                :src="`https://image.tmdb.org/t/p/w500${series.poster_path}`"
                :alt="series.name"
                class="movie-image"
                style="border-radius: 16px; overflow: hidden"
                width="250"
                height="100%"
                cover
              ></v-img>
              <div class="movie-overlay">
                <h3 class="movie-title">{{ series.name }}</h3>
                <p class="movie-release-date">
                  {{ series.first_air_date }}
                </p>
                <p class="movie-genres">
                  {{ mapGenres(series.genre_ids, seriesGenres) }}
                </p>
              </div>
            </div>
          </v-slide-item>
        </v-slide-group>
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
import axios from "axios";

export default {
  name: "HomeView",
  data() {
    return {
      movies: [],
      series: [],
      movieGenres: [],
      seriesGenres: [],
    };
  },
  mounted() {
    this.fetchPopularMovies();
    this.fetchTopSeries();
    this.fetchMovieGenres();
    this.fetchSeriesGenres();
  },
  methods: {
    async fetchPopularMovies() {
      try {
        const response = await axios.get("/api/fetch/popularMovies");
        this.movies = response.data;
      } catch (error) {
        console.error("Error fetching movies:", error);
      }
    },
    async fetchTopSeries() {
      try {
        const response = await axios.get("/api/fetch/topSeries");
        this.series = response.data;
      } catch (error) {
        console.error("Error fetching series:", error);
      }
    },
    async fetchMovieGenres() {
      try {
        const response = await axios.get("/api/fetch/genres");
        this.movieGenres = response.data;
      } catch (error) {
        console.error("Error fetching movie genres:", error);
      }
    },
    async fetchSeriesGenres() {
      try {
        const response = await axios.get("/api/fetch/seriesGenres");
        this.seriesGenres = response.data;
      } catch (error) {
        console.error("Error fetching series genres:", error);
      }
    },
    mapGenres(genreIds, genres) {
      if (!genreIds || !genres) return "";
      return genreIds
        .map((id) => {
          const genre = genres.find((g) => g.id === id);
          return genre ? genre.name : null;
        })
        .filter((g) => g !== null)
        .join(", ");
    },
  },
};
</script>

<style scoped>
.movie-image-wrapper {
  position: relative;
  display: inline-block;
  margin: 8px;
}

.movie-overlay {
  position: absolute;
  bottom: 0;
  left: 0;
  right: 0;
  background-color: rgba(0, 0, 0, 0.7);
  color: white;
  padding: 16px;
  opacity: 0;
  transition: opacity 0.3s ease-in-out;
  border-radius: 0 0 16px 16px;
}

.movie-image-wrapper:hover .movie-overlay {
  opacity: 1;
}

.movie-title {
  font-size: 1.4rem;
  font-weight: bold;
  margin: 0;
}

.movie-release-date {
  font-size: 0.9rem;
  margin: 8px 0;
}

.movie-genres {
  font-size: 0.9rem;
  margin: 8px 0;
}
</style>
