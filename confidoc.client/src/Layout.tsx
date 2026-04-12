import { Outlet } from "react-router-dom";
import Header from "./Header";
import Footer from "./Footer";
import DotBackground  from "./components/DotBackground"
function Layout() {
  return (
      <>
          <DotBackground />
          <Header />
          <Outlet />
          <Footer />
      </>
  );
}

export default Layout;