import { useAuthStore } from "../../store/authStore";
import { Link, Outlet, useLocation, useNavigate } from "react-router";
import { motion } from "framer-motion";
import { useEffect } from "react";

export default function Profile() {
  const { logout, id, fetchUser, User: user } = useAuthStore();
  const location = useLocation(); // Get the current path

  const isActive = (path) => location.pathname === path;

  const navigate = useNavigate();
  const signOut = async () => {
    await logout();
    navigate("/");
  };
  const getUser = async () => {
    await fetchUser(id);
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
      className="p-10 flex flex-col-reverse md:flex-row md:items-start items-end justify-end gap-10"
    >
      <Outlet />

      <div className="bg-gray-00  shadow-lg  h-fit justify-self-start text-end">
        <div className="bg-cyan-dark p-10 rounded-lg rounded-b-none  text-white flex flex-row-reverse  gap-2">
          <div className=" h-fit w-full rounded-full">
            <img className="size-18" src={user.photoUrl} alt="" />
          </div>

          <div>
            <p className="text-lg font-bold m-1">
              {user.firstName} {user.lastName}
            </p>
            <p className="m-1 overflow-clip">{user.email}</p>
            <p className="m-1">{user.phoneNumber}</p>
          </div>
        </div>
        <div className="p-10 pt-0">
          <ul>
            <li>
              <Link
                to={"/profile"}
                className={`m-b2 hover:text-cyan-dark ${
                  isActive("/profile") ? "font-bold " : ""
                }`}
              >
                حسابى
              </Link>
            </li>
            <li>
              <Link
                to={"/profile/tickets"}
                className={`m-b2 hover:text-cyan-dark ${
                  isActive("/profile/tickets") ? "font-bold " : ""
                }`}
              >
                التذاكر
              </Link>
            </li>
            <li className="mb-2 hover:text-cyan-dark cursor-pointer">
              <Link to={"change-password"}>تغيير كلمة السر</Link>
            </li>
            <li
              onClick={signOut}
              className="text-red-600 hover:text-red-800 cursor-pointer border-t-2 pt-4 border-dashed border-black "
            >
              تسجيل الخروج
            </li>
          </ul>
        </div>
      </div>
    </motion.div>
  );
}
