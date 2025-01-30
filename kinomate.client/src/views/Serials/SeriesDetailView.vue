<template>
    <div
      class="series-container"
      :style="`background-image: url('https://image.tmdb.org/t/p/original${series.backdrop_path}');`"
    >
      <v-container class="series-content">
        <v-row class="align-center">
          <v-col cols="12" md="3" class="d-flex justify-center">
            <v-img
              :src="`https://image.tmdb.org/t/p/original${series.poster_path}`"
              :alt="series.name"
              class="series-poster"
            ></v-img>
          </v-col>
          <v-col cols="12" md="7">
            <h1 class="text-h3 font-weight-bold text-white">{{ series.name }}</h1>
  
            <div class="series-details text-white">
              <p class="text-subtitle-1">
                <strong>First Air Date:</strong> {{ series.first_air_date }}
              </p>
              <p class="text-subtitle-1">
                <strong>Genres:</strong> {{ genreNames }}
              </p>
              <p class="text-subtitle-1">
                <strong>Rating by TMDb:</strong> {{ series.vote_average }}/10
              </p>
              <p class="text-subtitle-1">
                <strong>Rating by KinoMate:</strong> {{ averageRating }} /5
              </p>
              <p class="text-subtitle-1">
                <strong>Number of Seasons:</strong> {{ series.number_of_seasons }}
              </p>
            </div>
  
            <div class="series-overview mt-4 text-white">
              <h2 class="text-h5 font-weight-bold">Overview</h2>
              <p>{{ series.overview }}</p>
            </div>
  
            <v-row class="watch-providers-section mt-8">
              <v-col cols="12" md="8">
                <h2 class="text-h5 font-weight-bold text-white mb-4">Where to Watch</h2>
                <div v-if="watchProviders.length">
                  <v-chip
                    v-for="(provider, index) in watchProviders"
                    :key="index"
                    class="mr-2 mb-2"
                    color="primary"
                    outlined
                  >
                    <a
                      :href="provider.link"
                      target="_blank"
                      class="text-white text-decoration-none"
                    >
                      <v-img
                        :src="provider.logo"
                        alt="Provider logo"
                        max-height="24"
                        max-width="24"
                        class="mr-2"
                      />
                      {{ provider.name }}
                    </a>
                  </v-chip>
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
            <v-carousel v-if="series.trailerLinks && series.trailerLinks.length" hide-delimiters>
              <v-carousel-item v-for="(trailer, index) in series.trailerLinks" :key="index">
                <iframe
                  :src="`https://www.youtube.com/embed/${extractVideoId(trailer)}`"
                  frameborder="0"
                  allowfullscreen
                  class="trailer-iframe"
                ></iframe>
              </v-carousel-item>
            </v-carousel>
            <p v-else class="text-white text-center">Trailers not available</p>
          </v-col>
        </v-row>
      </v-container>
    </div>
  </template>
  
  <script>
  import httpClient from "@/plugins/httpClient";
  
  export default {
    name: "SeriesDetailView",
    data() {
      return {
        series: {},
        watchProviders: [],
      };
    },
    computed: {
      genreNames() {
        if (!this.series.genres || !this.series.genres.length) {
          return "No genres available";
        }
        return this.series.genres.map((genre) => genre.name).join(", ");
      },
      averageRating() {
        if (!this.series.comments || !this.series.comments.length) {
          return "No ratings available";
        }
        const total = this.series.comments.reduce((sum, comment) => sum + comment.rate, 0);
        return (total / this.series.comments.length).toFixed(1);
      },
    },
    mounted() {
      const seriesId = this.$route.params.id;
      this.fetchSeriesDetails(seriesId);
      this.fetchWatchProviders(seriesId);
    },
    methods: {
      async fetchSeriesDetails(seriesId) {
        try {
          const response = await httpClient.get(`/api/fetch/series/${seriesId}`);
          this.series = response.data;
        } catch (error) {
          console.error("Error fetching series details:", error);
        }
      },
      async fetchWatchProviders(seriesId) {
        try {
          const response = await httpClient.get(`/api/fetch/watchProviders/${seriesId}`);
          if (response.data && response.data.results && response.data.results.PL) {
            const providers = response.data.results.PL;
            this.watchProviders = (providers.flatrate || []).map((provider) => ({
              name: provider.provider_name,
              logo: `https://image.tmdb.org/t/p/w92${provider.logo_path}`,
              link: `https://www.justwatch.com/pl/serial/${this.series.name.replace(/\s+/g, '-').toLowerCase()}`,
            }));
          }
        } catch (error) {
          console.error("Error fetching watch providers:", error);
        }
      },
      extractVideoId(url) {
        const match = url.match(/(?:youtube\.com\/.*v=|youtu\.be\/)([^&]+)/);
        return match ? match[1] : "";
      },
    },
  };
  </script>
  
  <style scoped>
  .series-container {
    background-size: cover;
    background-position: center;
    background-repeat: no-repeat;
    padding-top: 75px;
  }
  .series-content {
    background: rgba(0, 0, 0, 0.6);
    padding: 40px;
    border-radius: 16px;
  }
  .series-poster {
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
  