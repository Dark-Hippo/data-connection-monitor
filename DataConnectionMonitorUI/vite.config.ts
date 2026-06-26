import { defineConfig, loadEnv } from "vite";
import react from "@vitejs/plugin-react";

// https://vitejs.dev/config/
export default defineConfig(({ command, mode }) => {
  const env = loadEnv(mode, process.cwd(), "");
  const port = Number(process.env.UI_PORT ?? env.UI_PORT) || 8080;
  const apiPort = Number(process.env.API_PORT ?? env.API_PORT) || 5000;
  const apiHost = process.env.API_HOST ?? env.API_HOST ?? "localhost";

  console.log("Vite config loaded", {
    cwd: process.cwd(),
    UI_PORT: process.env.UI_PORT ?? env.UI_PORT,
    API_PORT: process.env.API_PORT ?? env.API_PORT,
    API_HOST: apiHost,
    port,
    apiPort,
  });

  const proxy = {
    "/api": {
      target: `http://${apiHost}:${apiPort}`,
      changeOrigin: true,
    },
    "/disconnections-hub": {
      target: `ws://${apiHost}:${apiPort}`,
      changeOrigin: true,
      ws: true,
    },
  };

  return {
    plugins: [react()],
    preview: {
      port: port,
      strictPort: true,
      host: true,
      proxy: proxy,
    },
    server: {
      port: port,
      strictPort: true,
      host: true,
      proxy: proxy,
    },
  };
});
