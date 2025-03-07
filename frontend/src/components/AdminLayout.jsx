import Footer from "./Footer";
import { Outlet } from "react-router";
import AdminHeader from "./AdminHeader";

export default function AdminLayout() {
  return (
    <div className="flex flex-col h-screen">
      <AdminHeader />

      <div className="flex-auto">
        <Outlet />
      </div>

      <Footer />
    </div>
  );
}
