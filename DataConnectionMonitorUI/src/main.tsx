import React from 'react'
import ReactDOM from 'react-dom/client'
import App from './App.tsx'
import { BrowserRouter } from 'react-router-dom'

import "bootstrap/dist/css/bootstrap.min.css";
import './index.css'
import { ThemeProvider } from './contexts/ThemeContext.tsx';
import { TitleProvider } from './contexts/TitleContext.tsx';

ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <ThemeProvider>
      <TitleProvider>
        <BrowserRouter>
          <App />
        </BrowserRouter>
      </TitleProvider>
    </ThemeProvider>
  </React.StrictMode>
)
