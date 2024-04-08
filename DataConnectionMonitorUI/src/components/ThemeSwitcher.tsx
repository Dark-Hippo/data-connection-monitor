import { useTheme } from "../contexts/ThemeContext";
import { MoonStars } from "react-bootstrap-icons";

export const ThemeSwitcher = () => {
  const { toggleTheme } = useTheme();

  return (
    <div>
      <MoonStars style={{ width: "1em", height: "1em" }} onClick={toggleTheme} />
    </div>
  );
};
