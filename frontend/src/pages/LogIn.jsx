import { motion } from "framer-motion";
import { useForm } from "react-hook-form";
import { Link, useNavigate } from "react-router";
import { useAuthStore } from "../store/authStore";
import { Loader } from "lucide-react";
import Cookies from "js-cookie";
import { useEffect } from "react";
export default function LoginPage() {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm();
  const navigate = useNavigate();
  const { isLoading, error, login, setdefaulte } = useAuthStore();
  const onSubmit = async (data) => {
    console.log(data);
    try {
      const response = await login(data);
      console.log(response.roles[0]);
      const token = response.token;
      Cookies.set("token", token, { expires: 7, secure: true });
      console.log(error);
      if (response.roles[0] == "Admin") navigate("/admin/profile");
      else if (response.roles[0] == "Company") navigate("/company/profile");
      else navigate("/");
    } catch (error) {
      console.log(error.response.data);
    }
  };
  useEffect(() => {
    setdefaulte();
  }, []);
  return (
    <div className="size-full flex items-center">
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.5 }}
        className="flex flex-col items-center w-full"
      >
        <p className="text-4xl font-bold p-4">
          <span className="text-blue-500">Taz</span>
          <span className="text-orange-400">kartk</span>
        </p>
        <div className="w-2/3 mx-auto max-w-xl bg-white p-8 shadow-2xl rounded-lg text-end">
          <form onSubmit={handleSubmit(onSubmit)} className="">
            <h2 className="text-2xl text-cyan-dark font-bold mb-4 text-center">
              تسجيل الدخول
            </h2>

            <div className="mb-4">
              <label className="block text-gray-700 mb-2">
                البريد الإلكتروني
              </label>
              <input
                type="text"
                {...register("email", {
                  required: "يرجى إدخال البريد الالكترونى",
                })}
                className="w-full border border-gray-300 p-2 rounded-lg focus:outline-none focus:ring-2 focus:ring-teal-700"
              />
              {errors.email && (
                <p className="text-red-600 text-sm mt-1">
                  {errors.email.message}
                </p>
              )}
            </div>

            <div className="mb-4">
              <label className="block text-gray-700 mb-2">كلمة المرور</label>
              <input
                type="password"
                {...register("password", {
                  required: "يرجى إدخال كلمة المرور",
                })}
                className="w-full border border-gray-300 p-2 rounded-lg focus:outline-none focus:ring-2 focus:ring-teal-700"
              />
              {errors.password && (
                <p className="text-red-600 text-sm mt-1">
                  {errors.password.message}
                </p>
              )}
            </div>
            {error && (
              <p className="text-red-500 font-semibold mt-2">{error.message}</p>
            )}
            <motion.button
              whileHover={{ scale: 1.05 }}
              whileTap={{ scale: 0.95 }}
              type="submit"
              className="w-full mt-4 bg-cyan-dark text-white py-2 rounded-lg hover:bg-cyan-900"
            >
              {isLoading ? (
                <Loader className="animate-spin mx-auto" size={24} />
              ) : (
                " تسجيل الدخول"
              )}
            </motion.button>
          </form>
          <div className="flex flex-row-reverse pt-4">
            <Link className="text-blue-600" to={"/signup"}>
              {" "}
              نسيت كلمة المرور؟
            </Link>
          </div>
          <div className="flex flex-row-reverse pt-4">
            <p className="ml-2">ليس لديك حساب ؟ </p>
            <Link className="text-blue-600" to={"/signup"}>
              {" "}
              انشاء حساب
            </Link>
          </div>
        </div>
      </motion.div>
    </div>
  );
}
