import { BackArrow } from "./BackArrow";
import { Title } from "./Title";
import { ThemeSwitcher } from "./ThemeSwitcher";

export const Header = () => {
  return (
    <header>
      <BackArrow />
      <Title />
      <ThemeSwitcher />
    </header>
  );
}