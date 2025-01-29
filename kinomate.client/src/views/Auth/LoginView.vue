<template>
  <v-container fluid class="fill-height d-flex align-center justify-center">
    <v-row class="w-100">
      <v-col
        cols="12"
        md="6"
        class="d-flex flex-column justify-center align-center"
      >
        <div class="text-center px-8">
          <h1 class="display-2 font-weight-bold mb-4">Welcome to Kinomate</h1>
          <p class="text-h6 font-weight-light">
            Explore an extensive database of movies, from timeless classics to
            the latest blockbusters. Keep track of your favorites, discover new
            gems, and dive into detailed information about every film.
          </p>
        </div>
      </v-col>
      <v-col cols="12" md="6" class="d-flex justify-center align-center">
        <v-card width="70%">
          <v-toolbar dark>
            <v-toolbar-title>Login</v-toolbar-title>
          </v-toolbar>
          <v-card-text>
            <v-form @submit.prevent="login">
              <v-text-field
                v-model="username"
                label="Username or email"
                prepend-icon="mdi-account"
                type="text"
                required
              ></v-text-field>
              <v-text-field
                v-model="password"
                label="Password"
                prepend-icon="mdi-lock"
                type="password"
                required
              ></v-text-field>
              <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn color="primary" type="submit">Login</v-btn>
              </v-card-actions>
            </v-form>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
    <v-snackbar v-model="snackbar.show" top>
      {{ snackbar.text }}
    </v-snackbar>
  </v-container>
</template>

<script>
export default {
  data() {
    return {
      username: "",
      password: "",
      snackbar: {
        show: false,
        text: "",
      },
    };
  },
  methods: {
    async login() {
      try {
        await this.$store.dispatch("login", {
          username: this.username,
          password: this.password,
        });
        this.$router.push("/");
      } catch (error) {
        console.error("Login error:", error);
        this.snackbar.text = "Login error: " + error.message;
        this.snackbar.show = true;
      }
    },
  },
};
</script>

<style scoped>
.fill-height {
  height: 100vh;
}

.transparent-card {
  background-color: rgba(255, 255, 255, 0.85);
  border-radius: 16px;
  box-shadow: 0px 4px 10px rgba(0, 0, 0, 0.1);
}

.transparent-toolbar {
  background-color: transparent;
  padding: 0;
}

.semi-transparent-field {
  background-color: rgba(255, 255, 255, 0.7);
  border-radius: 8px;
}

.v-btn {
  border-radius: 24px;
}
</style>
