import { ReactNode, createContext, useContext, useState } from "react";

type TitleContextType = {
  title: string;
  setTitle: (title: string) => void;
};

const TitleContext = createContext<TitleContextType>({
  title: '',
  setTitle: () => { }
});

export const TitleProvider = ({ children }: { children: ReactNode }) => {
  const [title, setTitle] = useState('');

  return (
    <TitleContext.Provider value={{ title, setTitle }}>
      {children}
    </TitleContext.Provider>
  );
}

export const useTitle = () => {
  return useContext(TitleContext);
}