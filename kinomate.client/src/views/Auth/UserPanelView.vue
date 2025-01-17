<template>
  <v-container>
    <v-row>
      <v-col cols="4">
        <v-card>
          <v-card-title>User panel</v-card-title>
          <v-card-text>
            <v-list>
              <v-list-item> <b>Username:</b> {{ user.username }} </v-list-item>
              <v-list-item> <b>Email:</b> {{ user.email }} </v-list-item>
              <v-list-item>
                <v-btn @click="showChangePasswordDialog = true"
                  >Change Password</v-btn
                >
              </v-list-item>
            </v-list>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>

    <v-dialog v-model="showChangePasswordDialog" max-width="600px">
      <v-card>
        <v-card-title>
          <span class="headline">Change Password</span>
        </v-card-title>
        <v-card-text>
          <v-form ref="form">
            <v-text-field
              v-model="currentPassword"
              label="Current Password"
              type="password"
              required
            ></v-text-field>
            <v-text-field
              v-model="newPassword"
              label="New Password"
              type="password"
              required
            ></v-text-field>
          </v-form>
        </v-card-text>
        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn color="blue darken-1" text @click="changePassword"
            >Change</v-btn
          >
        </v-card-actions>
      </v-card>
    </v-dialog>
    <v-snackbar v-model="snackbar.show" top>
      {{ snackbar.text }}
    </v-snackbar>
  </v-container>
</template>

<script>
import axios from "axios";
import Cookies from "js-cookie";
export default {
  data() {
    return {
      user: { username: "", email: "" },
      showChangePasswordDialog: false,
      currentPassword: "",
      newPassword: "",
      snackbar: { show: false, color: "red", text: "" },
    };
  },
  created() {
    this.$store.dispatch("fetchUserInfo").then(() => {
      this.user = this.$store.state.user;
    });
  },
  methods: {
    initializeToken() {
      const token = Cookies.get("token");
      if (token) {
        this.$store.commit("setToken", token);
      }
    },
    async changePassword() {
      try {
        const token = this.$store.state.token;
        const response = await axios.post(
          "/api/changepassword",
          {
            currentPassword: this.currentPassword,
            newPassword: this.newPassword,
          },
          { headers: { Authorization: `Bearer ${token}` } }
        );
        if (response.status === 200) {
          this.showChangePasswordDialog = false;
          this.currentPassword = "";
          this.newPassword = "";
          this.snackbar.text = "Password has been changed ";
          this.snackbar.show = true;
          this.snackbar.color = "green";
        }
      } catch (error) {
        console.error("Password change error:", error);
        this.snackbar.text = "Password change error: " + error.message;
        this.snackbar.show = true;
        this.snackbar.color = "red";
      }
    },
  },
};
</script>

<style scoped>
.v-card, .v-card-title, .v-card-text, .v-list {
  background-color: rgba(0, 0, 0, 0.1);
}
</style>