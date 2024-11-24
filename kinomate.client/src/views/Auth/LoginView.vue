<template>
  <v-container>
    <v-row justify="center">
      <v-col cols="12" sm="8" md="8" >
        <v-card class="elevation-12">
          <v-toolbar color="primary" dark>
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
        this.snackbar.duration = 1000;
      }
    },
  },
};
</script>
