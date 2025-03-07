import { Link } from "react-router";
import logo from "../assets/file 1.png";
import { useAuthStore } from "../store/authStore";

export default function Header() {
  const { user, role } = useAuthStore();
  return (
    <header className="bg-cyan-dark text-white  flex justify-around">
      <nav className="flex flex-row-reverse  items-center grow-3 justify-around text-lg font-semibold ">
        <Link to={"/"} className="hover:text-gray-200">
          الرئيسية
        </Link>
        <a href="#" className="hover:text-gray-200">
          الشركات
        </a>
        <a href="#" className="hover:text-gray-200">
          من نحن
        </a>
        <a href="#" className="hover:text-gray-200">
          تواصل معنا
        </a>
        {!user ? (
          <Link
            to={"/login"}
            className="bg-white font-normal text-black p-2 rounded hover:bg-gray-300"
          >
            تسجيل الدخول
          </Link>
        ) : role === "user" ? (
          <Link
            to={"/user/profile"}
            className="bg-white font-normal text-black p-2 rounded hover:bg-gray-300"
          >
            حسابى
          </Link>
        ) : role === "admin" ? (
          <Link
            to={"/admin/profile"}
            className="bg-white font-normal text-black p-2 rounded hover:bg-gray-300"
          >
            حسابى
          </Link>
        ) : (
          <Link
            to={"/company/profile"}
            className="bg-white font-normal text-black p-2 rounded hover:bg-gray-300"
          >
            حسابى
          </Link>
        )}
      </nav>
      <div className="grow-2"></div>
      <div className="flex items-center justify-center  grow-1">
        <h1 className="text-2xl p-2 font-bold">Tazkartk</h1>
        <img className="w-20" src={logo} />
      </div>
    </header>
  );
}
