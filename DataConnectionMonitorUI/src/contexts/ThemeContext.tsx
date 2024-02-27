import { createContext, useContext, useState } from 'react';

import { ReactNode } from 'react';

interface Theme {
  theme: string;
  toggleTheme: () => void;
}

const defaultTheme: Theme = {
  theme: 'light',
  toggleTheme: () => { },
};

const ThemeContext = createContext<Theme>(defaultTheme);

export const ThemeProvider = ({ children }: { children: ReactNode }) => {
  const [theme, setTheme] = useState('light');

  const toggleTheme = () => {
    setTheme((prevTheme) => (prevTheme === 'light' ? 'dark' : 'light'));
  };

  return (
    <ThemeContext.Provider value={{ theme, toggleTheme }}>
      {children}
    </ThemeContext.Provider>
  );
}

export const useTheme = () => useContext(ThemeContext);