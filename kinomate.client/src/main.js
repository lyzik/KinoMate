import { createApp } from "vue";
import App from "./App.vue";
import router from "./router";
import store from "./store";
import vuetify from "./plugins/vuetify";
import { loadFonts } from "./plugins/webfontloader";
import Cookies from "js-cookie";

loadFonts();

const app = createApp(App);

app.config.globalProperties.$isLoggedIn = () => {
  return !!store.state.token || !!Cookies.get("token");
};

app
  .use(router)
  .use(store)
  .use(vuetify)
  .mount("#app");
