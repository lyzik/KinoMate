<template>
    <div>
      <v-container class="mt-12">
        <v-text-field
          v-model="searchQuery"
          label="Search for movies or series"
          append-icon="mdi-magnify"
          @keyup.enter="performSearch"
          @input="debounceSearch"
          clearable
          @click:clear="clearSearch"
        ></v-text-field>
        
        <v-row>
          <v-col cols="12" v-if="searchResults.length > 0">
            <h2 class="text-h5 font-weight-bold mb-4">Search Results</h2>
            <v-list>
              <v-list-item v-for="result in searchResults" :key="result.id">
                <router-link :to="generateLink(result)" class="text-decoration-none">
                  <v-list-item-title>{{ result.title }}</v-list-item-title>
                </router-link>
              </v-list-item>
            </v-list>
          </v-col>
          
          <v-col cols="12" v-else-if="searchQuery && searchResults.length === 0">
            <p class="text-center text-muted">No results found.</p>
          </v-col>
        </v-row>
      </v-container>
    </div>
  </template>
  
  <script>
  import axios from "axios";
  import debounce from "lodash/debounce";
  
  export default {
    name: "SearchView",
    data() {
      return {
        searchQuery: "",
        searchResults: [],
      };
    },
    methods: {
      async performSearch() {
        if (!this.searchQuery) return;
        try {
          const response = await axios.get(`/api/Fetch/search/${this.searchQuery}`);
          this.searchResults = response.data;
        } catch (error) {
          console.error("Error fetching search results:", error);
        }
      },
      clearSearch() {
        this.searchQuery = "";
        this.searchResults = [];
      },
      generateLink(result) {
        return result.type === "Movie" ? `/movies/${result.id}` : `/series/${result.id}`;
      },
      debounceSearch: debounce(function () {
        this.performSearch();
      }, 1100),
    },
  };
  </script>
  
  <style scoped>
  .text-decoration-none {
    text-decoration: none;
    color: inherit;
  }
  .text-muted {
    color: gray;
  }
  </style>