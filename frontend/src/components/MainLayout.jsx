import Header from "./Header";
import Footer from "./Footer";
import { Outlet } from "react-router";

export default function MainLayout() {
  return (
    <div className="flex flex-col h-screen">
      <Header />

      <div className="flex-auto">
        <Outlet />
      </div>

      <Footer />
    </div>
  );
}
