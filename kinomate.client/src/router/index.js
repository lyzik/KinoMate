import { createRouter, createWebHistory } from "vue-router";
import HomeView from "../views/HomeView.vue";
import LoginView from "@/views/Auth/LoginView.vue";
import UserPanelView from "@/views/Auth/UserPanelView.vue";
import RegisterView from "@/views/Auth/RegisterView.vue";
import Cookies from "js-cookie";
import SearchView from "@/views/SearchView.vue";
import MainPage from "@/views/Forum/MainPage.vue";

const requireAuth = (to, from, next) => {
  const isAuthenticated =
    Cookies.get("token") !== null && Cookies.get("token") !== undefined;
  if (isAuthenticated) {
    next();
  } else {
    next("/login");
  }
};

const routes = [
  {
    path: "/",
    name: "home",
    component: HomeView,
    beforeEnter: requireAuth,
  },
  {
    path: "/login",
    name: "login",
    component: LoginView,
  },
  {
    path: "/register",
    name: "register",
    component: RegisterView,
  },
  {
    path: "/user",
    name: "user",
    component: UserPanelView,
    beforeEnter: requireAuth,
  },
  {
    path: "/movies/:id",
    name: "movies-id",
    component: () => import("@/views/Movies/MovieDetailView.vue"),
    beforeEnter: requireAuth,
  },
  {
    path: "/series/:id",
    name: "series-id",
    component: () => import("@/views/Series/SeriesDetailView.vue"),
    beforeEnter: requireAuth,
  },
  {
    path: "/search",
    name: "search",
    component: SearchView,
    beforeEnter: requireAuth,
  },

  {
    path: "/Forum",
    name: "Forum",
    component: MainPage,
    beforeEnter: requireAuth,
  },

];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;
