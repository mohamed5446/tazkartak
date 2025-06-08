import { useAuthStore } from "../../store/authStore";
import { Link, Outlet, useLocation, useNavigate } from "react-router";
import { motion } from "framer-motion";
import { useEffect } from "react";

export default function Profile() {
  const { logout, id, fetchCompany, User: user } = useAuthStore();
  const location = useLocation(); // Get the current path

  const isActive = (path) => location.pathname === path;

  const navigate = useNavigate();
  const signOut = async () => {
    await logout();
    navigate("/");
  };
  const getUser = async () => {
    await fetchCompany(id);
  };
  useEffect(() => {
    getUser();
    console.log(user);
  }, [id]);
  return (
    <motion.div
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.5 }}
      className="p-2 flex flex-col-reverse xl:flex-row xl:items-start items-end justify-end gap-10"
    >
      <Outlet />

      <div className="bg-gray-100  shadow-lg  h-fit justify-self-start text-end">
        {user && user.logo && (
          <div className="bg-cyan-dark p-6 rounded text-white flex flex-row-reverse items-center gap-4">
            <div className="rounded overflow-hidden w-24 h-24 flex-shrink-0">
              <img
                className="object-cover w-full h-full"
                src={user.logo}
                alt="logo"
              />
            </div>
            <div className="text-sm">
              <p className="text-lg font-bold mb-1">{user.name}</p>
              <p className="truncate">{user.email}</p>
              <p>{user.phone}</p>
            </div>
          </div>
        )}

        <div className="p-10 pt-0">
          <ul className="flex gap-2 flex-col">
            <li>
              <Link
                to={"/company/profile"}
                className={` hover:text-cyan-dark ${
                  isActive("/company/profile") ? "font-bold " : ""
                }`}
              >
                حسابى
              </Link>
            </li>
            <li>
              <Link
                to={"/company/profile/trips"}
                className={` hover:text-cyan-dark ${
                  isActive("/company/profile/trips") ? "font-bold " : ""
                }`}
              >
                الرحلات
              </Link>
            </li>
            <li>
              <Link
                className={` hover:text-cyan-dark ${
                  isActive("/company/profile/change-password")
                    ? "font-bold "
                    : ""
                }`}
                to={"/company/profile/change-password"}
              >
                تغيير كلمة السر
              </Link>
            </li>
            <li>
              <Link
                className={` hover:text-cyan-dark ${
                  isActive("/company/profile/earnings") ? "font-bold " : ""
                }`}
                to={"/company/profile/earnings"}
              >
                الرصيد
              </Link>
            </li>
            <li
              onClick={signOut}
              className="text-red-600 hover:text-red-800 cursor-pointer border-t-2 pt-4 border-dashed border-black mt-4 "
            >
              تسجيل الخروج
            </li>
          </ul>
        </div>
      </div>
    </motion.div>
  );
}
