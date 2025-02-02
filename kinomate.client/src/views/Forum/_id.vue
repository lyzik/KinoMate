<template>
  <v-container class="mt-15">
    <v-card v-if="selectedPost">
      <v-card-title>{{ selectedPost.title }}</v-card-title>
      <v-card-text>
        <p>{{ selectedPost.description }}</p>
        <p>
          <strong>Utworzono:</strong> {{ formatDate(selectedPost.createdAt) }}
        </p>
        <!-- <p><strong>Polubienia:</strong> {{ selectedPost.likes }}</p> -->
        <h3>Komentarze:</h3>
        <v-list>
          <v-list-item
            v-for="comment in selectedPost.comments"
            :key="comment.id"
          >
            <v-list-item-content>
              <v-list-item-subtitle
                >{{ comment.username }} -
                {{ formatDate(comment.createdAt) }}</v-list-item-subtitle
              >
              <v-list-item-title>{{ comment.description }}</v-list-item-title>
            </v-list-item-content>
          </v-list-item>
        </v-list>

        <v-form @submit.prevent="addComment">
          <v-text-field
            v-model="newComment"
            label="Dodaj komentarz"
            outlined
            dense
            required
          ></v-text-field>
          <v-btn color="primary" type="submit">Dodaj</v-btn>
        </v-form>
      </v-card-text>
    </v-card>
  </v-container>
</template>

<script>
import httpClient from "@/plugins/httpClient";

export default {
  data() {
    return {
      dialog: false,
      postDialog: false,
      posts: [],
      selectedPost: null,
      newPost: {
        title: "",
        description: "",
      },
      newComment: "",
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
    fetchPostDetails() {
      httpClient
        .get(`/api/Forum/getPost/${this.$route.params.id}`)
        .then((response) => {
          this.selectedPost = response.data;
          this.postDialog = true;
        });
    },
    addComment() {
      console.log(this.newComment);
      httpClient
        .post(`/api/Forum/addComment/`, {
          postId: this.$route.params.id,
          description: this.newComment,
        })
        .then(() => {
          this.selectedPost.comments.push({
            description: this.newComment,
            createdAt: new Date().toISOString(),
          });
          this.newComment = "";
        })
        .catch((error) => {
          console.error("Failed to add comment:", error);
        });
    },
    formatDate(date) {
      const options = { year: "numeric", month: "numeric", day: "numeric" };
      return new Date(date).toLocaleDateString(undefined, options);
    },
  },
  created() {
    this.fetchPostDetails();
  },
};
</script>

<style scoped>
.font-weight-bold {
  font-weight: bold;
}
.text--secondary {
  color: grey;
}
</style>
