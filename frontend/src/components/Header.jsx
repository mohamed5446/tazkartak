import { Link, useLocation } from "react-router";
import logo from "../assets/file 1.png";
import { useAuthStore } from "../store/authStore";
import { Menu, X } from "lucide-react"; // Using Lucide React for icons
import { useState } from "react";

export default function Header() {
  const { user, role } = useAuthStore();
  const [isOpen, setIsOpen] = useState(false);
  const location = useLocation(); // Get the current path

  // Function to check if link is active
  const isActive = (path) => location.pathname === path;

  return (
    <header
      className={`top-0 z-50  bg-whit  bg-cyan-dark text-white px-4  md:px-8  w-full items-stretch  mx-auto ${
        isOpen
          ? "fixed h-screen items-stretch flex"
          : " sticky h-auto flex  justify-between items-center"
      }`}
    >
      {/* Mobile Menu Toggle */}
      <div className="md:hidden mr-auto mt-4 z-50">
        <button
          onClick={() => setIsOpen(!isOpen)}
          className="text-white text-3xl "
        >
          {isOpen ? <X size={36} /> : <Menu size={36} />}
        </button>
      </div>

      {/* Navigation Links */}
      <nav
        className={`absolute   items-center md:static top-0 left-0 w-full h-full md:w-auto bg-cyan-dark flex flex-col md:flex-row-reverse  justify-center text-lg font-semibold transition-all duration-300 ${
          isOpen
            ? "h-screen flex gap-6 text-2xl items-stretch text-center"
            : "hidden md:flex md:h-auto md:gap-6 items-stretch"
        }`}
      >
        <Link
          to={"/"}
          className={`p-2  md:flex items-center md:py-0 ${
            isActive("/")
              ? "text-white bg-cyan-900 py-4 md:border-none"
              : "hover:text-gray-200"
          }`}
          onClick={() => setIsOpen(false)}
        >
          الرئيسية
        </Link>
        <a
          href="#"
          className="hover:text-gray-200 md:flex items-center py-2 md:py-0"
          onClick={() => setIsOpen(false)}
        >
          الشركات
        </a>
        <Link
          to={"/about"}
          className={`p-2  md:flex items-center md:py-0 ${
            isActive("/about")
              ? "text-white bg-cyan-900  py-4 md:border-none"
              : "hover:text-gray-200"
          }`}
          onClick={() => setIsOpen(false)}
        >
          من نحن
        </Link>
        <Link
          to={"/contact-us"}
          className={`p-2  md:flex items-center md:py-0 ${
            isActive("/contact-us")
              ? "text-white bg-cyan-900 py-4 md:border-none"
              : "hover:text-gray-200"
          }`}
          onClick={() => setIsOpen(false)}
        >
          تواصل معنا
        </Link>

        {/* Auth Links */}
        {!user ? (
          <Link
            to={"/login"}
            className="bg-white w-fit self-center text-black px-6 py-2 rounded hover:bg-gray-300"
            onClick={() => setIsOpen(false)}
          >
            تسجيل الدخول
          </Link>
        ) : (
          <Link
            to={
              role === "User"
                ? "/user/profile"
                : role === "Admin"
                ? "/admin/profile"
                : "/company/profile"
            }
            className={`bg-white w-fit self-center text-black px-6 py-2 rounded hover:bg-gray-300 ${
              isActive("/user/profile") ||
              isActive("/admin/profile") ||
              isActive("/company/profile")
                ? "border-2 border-cyan-dark"
                : ""
            }`}
            onClick={() => setIsOpen(false)}
          >
            حسابى
          </Link>
        )}
      </nav>
      {/* Logo & Site Name */}
      <Link to={"/"}>
        <div className={`flex items-center ${isOpen ? "hidden" : ""}`}>
          <img className="w-16 md:w-16" src={logo} alt="Tazkartk Logo" />
          <h1 className="text-xl md:text-2xl font-bold ml-2">Tazkartk</h1>
        </div>
      </Link>
    </header>
  );
}
