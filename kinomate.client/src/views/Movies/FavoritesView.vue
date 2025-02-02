<template>
  <v-container fluid class="mt-16">
    <v-row>
      <v-col cols="12">
        <h2 class="text-h4 font-weight-bold mb-6 ml-4">Favorite Movies</h2>
      </v-col>
    </v-row>
    <v-row>
      <v-col
        v-for="movie in favoriteMovies"
        :key="movie.id"
        cols="12"
        sm="6"
        md="4"
        lg="3"
      >
        <div class="movie-card">
          <router-link :to="`/movies/${movie.id}`">
            <v-img
              :src="`https://image.tmdb.org/t/p/w500${movie.poster}`"
              :alt="movie.title"
              class="movie-poster"
              cover
            ></v-img>
          </router-link>
          <div class="movie-info">
            <h3 class="movie-title">{{ movie.title }}</h3>
          </div>
        </div>
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
import httpClient from "@/plugins/httpClient";

export default {
  name: "FavoriteMoviesView",
  data() {
    return {
      favoriteMovies: [],
    };
  },
  mounted() {
    this.fetchFavoriteMovies();
  },
  methods: {
    async fetchFavoriteMovies() {
      try {
        const response = await httpClient.get("/api/fetch/getFavoriteMovies");
        this.favoriteMovies = response.data;
      } catch (error) {
        console.error("Error fetching favorite movies:", error);
      }
    },
  },
};
</script>

<style scoped>
.movie-card {
  position: relative;
  margin: 8px;
  border-radius: 16px;
  overflow: hidden;
  transition: transform 0.3s ease-in-out;
}

.movie-card:hover {
  transform: scale(1.05);
}

.movie-poster {
  border-radius: 16px;
}

.movie-info {
  text-align: center;
  margin-top: 8px;
}

.movie-title {
  font-size: 1.2rem;
  font-weight: bold;
  color: #333;
}
</style>
