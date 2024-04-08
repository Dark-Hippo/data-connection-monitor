import { defineConfig, loadEnv } from "vite";
import react from "@vitejs/plugin-react";

// https://vitejs.dev/config/
export default defineConfig(({ command, mode }) => {
  const env = loadEnv(mode, process.cwd());
  const port = Number(env.UI_PORT) || 8080;
  const apiPort = Number(env.API_PORT) || 5000;
  return {
    plugins: [react()],
    preview: {
      port: port,
      strictPort: true,
      host: true,
      proxy: {
        "^/api/*": {
          target: `http://localhost:${apiPort}`,
          changeOrigin: true,
        },
        "^/disconnections-hub": {
          target: `http://localhost:${apiPort}`,
          changeOrigin: true,
          ws: true,
        },
      },
    },
    server: {
      port: port,
      strictPort: true,
      host: true,
      proxy: {
        "^/api/*": {
          target: `http://localhost:${apiPort}`,
          changeOrigin: true,
        },
        "^/disconnections-hub": {
          target: `http://localhost:${apiPort}`,
          changeOrigin: true,
          ws: true,
        },
      },
    },
  };
});
