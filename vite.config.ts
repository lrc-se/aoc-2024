import { defineConfig } from "vite";
import vue from "@vitejs/plugin-vue";

export default defineConfig(({ command }) => ({
  plugins: [vue()],
  define: {
    API_HOST: JSON.stringify(command === "serve" ? `http://localhost:${process.env.port || 1337}` : "")
  }
}));
