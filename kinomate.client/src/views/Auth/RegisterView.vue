<template>
  <v-container>
    <v-row justify="center">
      <v-col cols="12" sm="8" md="8">
        <v-card>
          <v-toolbar color="primary" dark>
            <v-toolbar-title>Register</v-toolbar-title>
          </v-toolbar>
          <v-card-text>
            <v-form ref="form" v-model="formValid" @submit.prevent="register">
              <v-text-field
                v-model="email"
                label="Email"
                prepend-icon="mdi-email"
                type="email"
                :rules="emailRules"
                required
              ></v-text-field>
              <v-text-field
                v-model="username"
                label="Username"
                prepend-icon="mdi-account"
                type="text"
                :rules="usernameRules"
                required
              ></v-text-field>
              <v-text-field
                v-model="password"
                label="Password"
                prepend-icon="mdi-lock"
                type="password"
                :rules="passwordRules"
                required
              ></v-text-field>
              <v-text-field
                v-model="confirmPassword"
                label="Confirm Password"
                prepend-icon="mdi-lock"
                type="password"
                :rules="[confirmPasswordRule(password)]"
                required
              ></v-text-field>
              <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn :disabled="!formValid" color="primary" type="submit">Register</v-btn>
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
import {
  emailRules,
  usernameRules,
  passwordRules,
  confirmPasswordRule,
} from "@/utils/validationRules";

export default {
  data() {
    return {
      username: "",
      password: "",
      confirmPassword: "",
      email: "",
      snackbar: {
        show: false,
        text: "",
      },
      formValid: false,
    };
  },
  computed: {
    emailRules() {
      return emailRules;
    },
    usernameRules() {
      return usernameRules;
    },
    passwordRules() {
      return passwordRules;
    },
    confirmPasswordRule() {
      return confirmPasswordRule(this.password);
    },
  },
  methods: {
    async register() {
      if (this.$refs.form.validate()) {
        if (this.password !== this.confirmPassword) {
          this.snackbar.text = "Passwords do not match!";
          this.snackbar.show = true;
          return;
        }
        try {
          const response = await fetch("/api/register", {
            method: "POST",
            headers: {
              "Content-Type": "application/json",
            },
            body: JSON.stringify({
              email: this.email,
              username: this.username,
              password: this.password,
            }),
          });
          if (response.ok) {
            this.snackbar.text = "Registration successful!";
            this.snackbar.show = true;

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
          } else {
            const errorData = await response.json();
            this.snackbar.text = "Register error: " + errorData.message;
            this.snackbar.show = true;
          }
        } catch (error) {
          console.error("Register error:", error);
          this.snackbar.text = "Register error: " + error.message;
          this.snackbar.show = true;
        }
      }
    },
  },
};
</script>
