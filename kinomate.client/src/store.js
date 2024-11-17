import { createStore } from "vuex";
import Cookies from "js-cookie";

export default createStore({
  state: {
    token: Cookies.get("token") || null,
    user: Cookies.get("user") ? JSON.parse(Cookies.get("user")) : null,
  },
  mutations: {
    setToken(state, token) {
      state.token = token;
      if (token) {
        Cookies.set("token", token, { expires: 7, secure: true });
      } else {
        Cookies.remove("token");
      }
    },
    setUser(state, user) {
      state.user = user;
      if (user) {
        Cookies.set("user", JSON.stringify(user), { expires: 7, secure: true });
      } else {
        Cookies.remove("user");
      }
    },
  },
  actions: {
    async login({ commit }, { username, password }) {
      try {
        const response = await fetch("/api/login", {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({ username, password }),
        });

        if (response.ok) {
          const data = await response.json();
          const token = data.token;
          const user = data.user;
          commit("setToken", token);
          commit("setUser", user);
        } else {
          throw new Error("Login failed");
        }
      } catch (error) {
        console.error("Login error:", error);
        throw error;
      }
    },
    logout({ commit }) {
      commit("setToken", null);
      commit("setUser", null);
    },
    async fetchUserInfo({ state, commit }) {
      const token = state.token || Cookies.get("token");

      if (!token) {
        throw new Error("No token for authorization.");
      }

      try {
        const response = await fetch("/api/userinfo", {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        });

        if (response.ok) {
          const userData = await response.json();
          commit("setUser", userData);
        } else {
          throw new Error("Failed to fetch user data.");
        }
      } catch (error) {
        console.error("Error fetching user information:", error);
      }
    },
  },
  getters: {
    isAuthenticated(state) {
      return !!state.token;
    },
    getUser(state) {
      return state.user;
    },
  },
});
