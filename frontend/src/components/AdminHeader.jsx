import { Link } from "react-router";
import logo from "../assets/file 1.png";
import { useAuthStore } from "../store/authStore";

export default function AdminHeader() {
  const { user } = useAuthStore();
  return (
    <header className="bg-cyan-dark text-white  flex justify-between">
      <nav className="flex flex-row-reverse  items-center grow-3 justify-end text-lg font-semibold ">
        {!user ? (
          <Link
            to={"/login"}
            className="bg-white font-normal text-black p-2 m-2 rounded hover:bg-gray-300"
          >
            تسجيل الدخول
          </Link>
        ) : (
          <Link
            to={"/admin/profile"}
            className="bg-white font-normal text-black p-2 m-2 rounded hover:bg-gray-300"
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
