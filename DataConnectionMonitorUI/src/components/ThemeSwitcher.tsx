import { useTheme } from "../contexts/ThemeContext";
import { MoonStars } from "react-bootstrap-icons";

export const ThemeSwitcher = () => {
  const { toggleTheme } = useTheme();

  return (
    <div>
      <MoonStars style={{ width: "0.75em", height: "0.75em" }} onClick={toggleTheme} />
    </div>
  );
};
