<template>
  <v-container class="mt-15">
    <v-text-field
      v-model="search"
      label="Szukaj"
      @input="fetchPosts"
    ></v-text-field>

    <v-btn @click="dialog = true" class="mb-4">Dodaj Post</v-btn>

    <v-dialog v-model="dialog" max-width="500px">
      <v-card>
        <v-card-title>
          <span class="headline">Dodaj Post</span>
        </v-card-title>
        <v-card-text>
          <v-form ref="form">
            <v-text-field v-model="newPost.title" label="Tytuł" required></v-text-field>
            <v-textarea v-model="newPost.description" label="Opis" required></v-textarea>
          </v-form>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="blue darken-1" text @click="dialog = false">Anuluj</v-btn>
          <v-btn color="blue darken-1" text @click="addPost">Dodaj</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>

    <v-progress-linear v-if="isLoading" indeterminate color="blue darken-1"></v-progress-linear>

    <v-list>
      <v-list-item v-for="post in posts" :key="post.id" @click="goToPost(post.id)" class="clickable">
        <v-list-item-content>
          <v-list-item-title class="font-weight-bold mb-2">
            {{ post.title }}
          </v-list-item-title>
          <v-list-item-subtitle class="text--secondary mb-2">
            Created by: {{ post.username }} | {{ formatDate(post.createdAt) }}
          </v-list-item-subtitle>
        </v-list-item-content>
        <!-- <v-list-item-action class="mb-4">
          <v-icon class="mr-3">mdi-thumb-up</v-icon>
          <span>{{ post.likes }}</span>
        </v-list-item-action> -->
        <v-divider></v-divider>
      </v-list-item>
    </v-list>

    <v-pagination v-model="page" :length="totalPages" @update:model-value="fetchPosts"></v-pagination>
  </v-container>
</template>

<script>
import httpClient from "@/plugins/httpClient";

export default {
  data() {
    return {
      dialog: false,
      posts: [],
      newPost: {
        title: "",
        description: "",
      },
      page: 1,
      pageSize: 6,
      totalPages: 1,
      search: "",
      isLoading: false,
    };
  },

  methods: {
    fetchPosts() {
      this.isLoading = true;
      httpClient
        .get("/api/Forum/getPosts", {
          params: {
            page: this.page,
            pageSize: this.pageSize,
            search: this.search,
          },
        })
        .then((response) => {
          this.posts = response.data.posts;
          this.totalPages = response.data.totalPages;
        })
        .finally(() => {
          this.isLoading = false;
        });
    },
    addPost() {
      httpClient.post("/api/Forum/addPost", this.newPost).then(() => {
        this.dialog = false;
        this.newPost.title = "";
        this.newPost.description = "";
        this.fetchPosts();
      });
    },
    formatDate(date) {
      const options = { year: "numeric", month: "numeric", day: "numeric" };
      return new Date(date).toLocaleDateString(undefined, options);
    },
    goToPost(postId) {
      this.$router.push(`/forum/${postId}`);
    },
  },
  created() {
    this.fetchPosts();
  },
};
</script>

<style scoped>
.clickable {
  cursor: pointer;
}
.font-weight-bold {
  font-weight: bold;
}
.text--secondary {
  color: grey;
}
</style>
