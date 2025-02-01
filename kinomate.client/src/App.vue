<template>
  <v-app>
    <v-app-bar app :color="'rgba(0, 0, 0, 0.0)'" class="pl-2" flat>
      <v-toolbar-title class="d-flex align-center">
        <router-link to="/" class="logo-link">
          <v-img
            src="@/assets/LogoHeader.png"
            alt="Logo"
            contain
            width="200px"
            class="logo-img"
          ></v-img>
        </router-link>
      </v-toolbar-title>
      <v-spacer></v-spacer>
      <template v-if="isLoggedIn">
        <v-btn text to="/forum">Forum</v-btn>
        <v-btn text to="/search">Search</v-btn>
        <v-btn text to="/user">
          <v-icon left>mdi-account</v-icon>
          {{ username }}
        </v-btn>
        <v-btn text @click="logout">Logout</v-btn>
      </template>
      <template v-else>
        <v-btn text to="/login">Login</v-btn>
        <v-btn text to="/register">Register</v-btn>
      </template>
    </v-app-bar>
    <v-main class="custom-main">
      <router-view />
    </v-main>
  </v-app>
</template>

<script>
import { mapState, mapActions } from "vuex";
import Cookies from "js-cookie";

export default {
  name: "App",
  mounted() {
    document.title = "Kinomate";
  },
  computed: {
    ...mapState(["token", "user"]),
    isLoggedIn() {
      return !!this.token || !!Cookies.get("token");
    },
    username() {
      const user = this.user || JSON.parse(Cookies.get("user") || "{}");
      return user.username || "Unknown";
    },
  },
  created() {
    const token = Cookies.get("token");
    if (token && !this.token) {
      this.$store.commit("setToken", token);
    }

    const userCookie = Cookies.get("user");
    if (userCookie && !this.user) {
      const user = JSON.parse(userCookie);
      this.$store.commit("setUser", user);
    }

    if (this.isLoggedIn && !this.user) {
      this.fetchUserInfo();
    }
  },
  methods: {
    ...mapActions(["fetchUserInfo", "logout"]),
    logout() {
      this.$store.dispatch("logout");
      this.$router.push("/login");
    },
  },
};
</script>
<style>
.custom-main {
  margin-top: -64px;
}
</style>
