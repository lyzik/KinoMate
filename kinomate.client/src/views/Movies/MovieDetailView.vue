<template>
    <div>
      <v-container>
        <v-row class="mt-6">
          <v-col cols="12" md="4">
            <v-img
              :src="`https://image.tmdb.org/t/p/original${movie.poster_path}`"
              :alt="movie.title"
              class="movie-poster"
              style="border-radius: 16px; overflow: hidden"
              width="100%"
              height="auto"
              cover
            ></v-img>
          </v-col>
  
          <v-col cols="12" md="8">
            <h1 class="text-h3 font-weight-bold">{{ movie.title }}</h1>
  
            <div class="movie-details">
              <p class="text-subtitle-1">
                <strong>Release Date:</strong> {{ movie.release_date }}
              </p>
              <p class="text-subtitle-1">
                <strong>Genres:</strong> {{ mapGenres(movie.genre_ids, movieGenres) }}
              </p>
              <p class="text-subtitle-1">
                <strong>Rating:</strong> {{ movie.vote_average }}/10
              </p>
              <p class="text-subtitle-1">
                <strong>Runtime:</strong> {{ movie.runtime }} mins
              </p>
            </div>
  
            <div class="movie-overview mt-4">
              <h2 class="text-h5 font-weight-bold">Overview</h2>
              <p>{{ movie.overview }}</p>
            </div>
          </v-col>
        </v-row>
      </v-container>
    </div>
  </template>
  
  <script>
  import axios from "axios";
  
  export default {
    name: "MovieDetailView",
    data() {
      return {
        movie: {},
        movieGenres: [],
      };
    },
    mounted() {
      const movieId = this.$route.params.id; // Assumes you are using Vue Router
      this.fetchMovieDetails(movieId);
      this.fetchMovieGenres();
    },
    methods: {
      async fetchMovieDetails(movieId) {
        try {
          const response = await axios.get(`/api/fetch/movie/${movieId}`);
          this.movie = response.data;
        } catch (error) {
          console.error("Error fetching movie details:", error);
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
  .movie-poster {
    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
  }
  .movie-details p {
    margin: 8px 0;
  }
  .movie-overview {
    line-height: 1.6;
  }
  </style>
  