<template>
  <div
    class="movie-container"
    :style="`background-image: url('https://image.tmdb.org/t/p/original${movie.backdrop_path}');`"
  >
    <v-container class="movie-content">
      <v-row class="align-center">
        <v-col cols="12" md="3" class="d-flex justify-center">
          <v-img
            :src="`https://image.tmdb.org/t/p/original${movie.poster_path}`"
            :alt="movie.title"
            class="movie-poster"
          ></v-img>
        </v-col>
        <v-col cols="12" md="7">
          <h1 class="text-h3 font-weight-bold text-white">{{ movie.title }}</h1>
          <div class="d-flex align-center mb-3">
            <v-btn class="mr-3 my-5" icon @click="toggleFavorite" style="background-color: transparent;">
              <v-icon :color="movie.isFavorite ? 'red' : 'white'">
                {{ movie.isFavorite ? "mdi-heart" : "mdi-heart-outline" }}
              </v-icon>
            </v-btn>
            <v-btn icon @click="toggleNotification"  style="background-color: transparent;">
              <v-icon :color="movie.hasNotification ? 'yellow' : 'white'">
                {{ movie.hasNotification ? "mdi-bell" : "mdi-bell-outline" }}
              </v-icon>
            </v-btn>
          </div>
          <div class="movie-details text-white">
            <p class="text-subtitle-1">
              <strong>Release Date:</strong> {{ movie.release_date }}
            </p>
            <p class="text-subtitle-1">
              <strong>Genres:</strong> {{ genreNames }}
            </p>
            <p class="text-subtitle-1">
              <strong>Rating by TMDb:</strong> {{ movie.vote_average }}/10
            </p>
            <p class="text-subtitle-1">
              <strong>Rating by KinoMate:</strong> {{ averageRating }} /5
            </p>
            <p class="text-subtitle-1">
              <strong>Runtime:</strong> {{ movie.runtime }} mins
            </p>
          </div>

          <div class="movie-overview mt-4 text-white">
            <h2 class="text-h5 font-weight-bold">Overview</h2>
            <p>{{ movie.overview }}</p>
          </div>
          <v-row class="watch-providers-section mt-2">
            <v-col>
              <h2 class="text-h5 font-weight-bold text-white mb-2">
                Where to Watch
              </h2>
              <div
                v-if="
                  movie.streamingLink &&
                  movie.streamingPlatforms &&
                  movie.streamingPlatforms.length
                "
              >
                <div class="d-flex align-center">
                  <span
                    v-for="(platform, index) in movie.streamingPlatforms"
                    :key="index"
                    class="mr-2"
                  >
                    <v-img
                      :src="platform.logo_url"
                      :alt="platform.name"
                      width="50px"
                    ></v-img
                  ></span>

                  <a
                    :href="movie.streamingLink"
                    target="_blank"
                    class="text-white"
                    >Links on TMDb</a
                  >
                </div>
              </div>
              <p v-else class="text-white">No streaming options available.</p>
            </v-col>
          </v-row>
        </v-col>
      </v-row>
      <v-row class="trailer-carousel-container" justify="center">
        <v-col cols="12" md="8">
          <h2 class="text-h5 font-weight-bold text-white mb-5 text-center">
            Watch Trailers
          </h2>
          <v-carousel
            v-if="movie.trailerLinks && movie.trailerLinks.length"
            hide-delimiters
          >
            <v-carousel-item
              v-for="(trailer, index) in movie.trailerLinks"
              :key="index"
            >
              <iframe
                :src="`https://www.youtube.com/embed/${extractVideoId(
                  trailer
                )}`"
                frameborder="0"
                allowfullscreen
                class="trailer-iframe"
              ></iframe>
            </v-carousel-item>
          </v-carousel>
          <p v-else class="text-white text-center">Trailers not available</p>
        </v-col>
      </v-row>
      <v-row class="mt-8">
        <v-col cols="12" md="8">
          <h2 class="text-h5 font-weight-bold text-white mb-4">
            User Comments
          </h2>
          <v-card
            v-for="(comment, index) in movie.comments"
            :key="index"
            class="mb-4"
            outlined
          >
            <v-card-text>
              <div class="d-flex align-center">
                <p class="text-subtitle-1 mb-0 mr-3">
                  <strong>{{ comment.username }}</strong>
                </p>
                <v-rating
                  v-model="comment.rate"
                  background-color="grey lighten-1"
                  color="yellow"
                  readonly
                  size="x-small 20"
                ></v-rating>
              </div>
              <p>{{ comment.commentText }}</p>
              <p class="text-caption text-grey">
                Posted on: {{ new Date(comment.createdAt).toLocaleString() }}
              </p>
            </v-card-text>
          </v-card>

          <h2 class="text-h5 font-weight-bold text-white mt-6">
            Add a Comment
          </h2>
          <v-form @submit.prevent="submitComment">
            <v-textarea
              v-model="newComment.text"
              label="Your Comment"
              outlined
              required
            ></v-textarea>
            <v-row>
              <v-rating
                v-model.number="newComment.rate"
                background-color="grey lighten-1"
                color="yellow"
                small
                half-increments
                length="5"
                label="Rating (1-5)"
                required
              ></v-rating>
              <v-btn flat type="submit" class="mt-2">Submit</v-btn>
            </v-row>
          </v-form>
        </v-col>
      </v-row>
    </v-container>
  </div>
</template>

<script>
import httpClient from "@/plugins/httpClient";

export default {
  name: "MovieDetailView",
  data() {
    return {
      movie: {},
      movieGenres: [],
      newComment: {
        text: "",
        rate: null,
      },
    };
  },
  computed: {
    genreNames() {
      if (!this.movie.genres || !this.movie.genres.length) {
        return "No genres available";
      }
      return this.movie.genres.map((genre) => genre.name).join(", ");
    },
    averageRating() {
      if (!this.movie.comments || !this.movie.comments.length) {
        return "No ratings available";
      }
      const total = this.movie.comments.reduce(
        (sum, comment) => sum + comment.rate,
        0
      );
      return (total / this.movie.comments.length).toFixed(1);
    },
  },
  mounted() {
    const movieId = this.$route.params.id;
    this.fetchMovieDetails(movieId);
    this.fetchMovieGenres();
  },
  methods: {
    async fetchMovieDetails(movieId) {
      try {
        const response = await httpClient.get(`/api/fetch/movie/${movieId}`);
        this.movie = response.data;
      } catch (error) {
        console.error("Error fetching movie details:", error);
      }
    },
    async fetchMovieGenres() {
      try {
        const response = await httpClient.get("/api/fetch/genres");
        this.movieGenres = response.data;
      } catch (error) {
        console.error("Error fetching movie genres:", error);
      }
    },
    async submitComment() {
      try {
        const payload = {
          movieId: this.movie.id,
          commentText: this.newComment.text,
          rate: this.newComment.rate,
          createdAt: new Date().toISOString(),
          mediaType: 0,
        };

        await httpClient.post("/api/data/addComment", payload);
        payload.username = this.$store.state.user.username;
        this.movie.comments.unshift(payload);
        this.newComment.text = "";
        this.newComment.rate = null;
      } catch (error) {
        console.error("Error submitting comment:", error);
      }
    },
    extractVideoId(url) {
      const match = url.match(/(?:youtube\.com\/.*v=|youtu\.be\/)([^&]+)/);
      return match ? match[1] : "";
    },
    async toggleFavorite() {
      try {
        await httpClient.post(`/api/Data/toggleFavoriteMovie/${this.movie.id}`);
        this.movie.isFavorite = !this.movie.isFavorite;
      } catch (error) {
        console.error("Error toggling favorite:", error);
      }
    },
    async toggleNotification() {
      try {
        await httpClient.post(
          `/api/Data/toggleMovieNotification/${this.movie.id}`
        );
        this.movie.hasNotification = !this.movie.hasNotification;
      } catch (error) {
        console.error("Error toggling notification:", error);
      }
    },
  },
};
</script>

<style scoped>
.movie-container {
  background-size: cover;
  background-position: center;
  background-repeat: no-repeat;
  padding-top: 75px;
}

.movie-content {
  background: rgba(0, 0, 0, 0.6);
  padding: 40px;
  border-radius: 16px;
}

.movie-poster {
  width: 100%;
  max-width: 300px;
  border-radius: 16px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.3);
}

.trailer-carousel-container {
  margin-top: 20px;
}

.trailer-iframe {
  width: 100%;
  height: 100%;
  border-radius: 16px;
}
</style>
