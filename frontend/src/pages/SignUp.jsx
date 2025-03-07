import { motion } from "framer-motion";
import { useForm } from "react-hook-form";
import { Link, useNavigate } from "react-router";
import { useAuthStore } from "../store/authStore";
import { Loader } from "lucide-react";
import { useEffect } from "react";
export default function SignUpPage() {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm();

  const { userSignup, adminSignup, error, isLoading, setdefaulte } =
    useAuthStore();

  const navigate = useNavigate();

  const onSubmit = async (data) => {
    const { accountType, ...formData } = data;

    console.log(accountType);
    try {
      if (accountType === "user") {
        await userSignup(formData);
      } else if (accountType == "admin") {
        await adminSignup(formData);
      }

      navigate("/verify-email");
    } catch (error) {
      console.log(error.response);
    }
  };
  useEffect(() => {
    setdefaulte();
  }, []);
  return (
    <div className="size-full flex flex-col ">
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.5 }}
        className="w-full m-auto"
      >
        <p className="text-4xl font-bold text-center p-4">
          <span className="text-blue-500">Taz</span>
          <span className="text-orange-400">kartk</span>
        </p>
        <div className="w-2/3 mx-auto max-w-xl bg-white p-8 shadow-2xl rounded-lg text-end m-4">
          <h2 className="text-2xl text-cyan-dark font-bold mb-6 text-center">
            إنشاء حساب
          </h2>
          <form onSubmit={handleSubmit(onSubmit)}>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label className="block text-gray-700 mb-2">اسم العائلة </label>
                <input
                  type="text"
                  {...register("lastName", {
                    required: "يرجى إدخال اسم العائلة",
                  })}
                  className="w-full border border-gray-300 p-2 rounded-lg focus:outline-none focus:ring-2 focus:ring-teal-700"
                />
                {errors.lastName && (
                  <p className="text-red-600 text-sm">
                    {errors.lastName.message}
                  </p>
                )}
              </div>
              <div>
                <label className="block text-gray-700 mb-2">الاسم الاول</label>
                <input
                  type="text"
                  {...register("firstName", {
                    required: "يرجى إدخال الاسم الاول",
                  })}
                  className="w-full border border-gray-300 p-2 rounded-lg focus:outline-none focus:ring-2 focus:ring-teal-700"
                />
                {errors.firstName && (
                  <p className="text-red-600 text-sm">
                    {errors.firstName.message}
                  </p>
                )}
              </div>

              <div>
                <label className="block text-gray-700 mb-2">رقم الهاتف</label>
                <input
                  type="text"
                  {...register("phoneNumber", {
                    required: "يرجى إدخال رقم الهاتف",
                  })}
                  className="w-full border border-gray-300 p-2 rounded-lg focus:outline-none focus:ring-2 focus:ring-teal-700"
                />
                {errors.phoneNumber && (
                  <p className="text-red-600 text-sm">
                    {errors.phoneNumber.message}
                  </p>
                )}
              </div>

              <div>
                <label className="block text-gray-700 mb-2">
                  البريد الإلكتروني
                </label>
                <input
                  type="email"
                  {...register("email", {
                    required: "يرجى إدخال البريد الإلكتروني",
                  })}
                  className="w-full border border-gray-300 p-2 rounded-lg focus:outline-none focus:ring-2 focus:ring-teal-700"
                />
                {errors.email && (
                  <p className="text-red-600 text-sm">{errors.email.message}</p>
                )}
              </div>

              <div className="col-span-2">
                <label className="block text-gray-700 mb-2">كلمة المرور</label>
                <input
                  type="password"
                  {...register("password", {
                    required: "يرجى إدخال كلمة المرور",
                  })}
                  className="w-full border border-gray-300 p-2 rounded-lg focus:outline-none focus:ring-2 focus:ring-teal-700"
                />
                {errors.password && (
                  <p className="text-red-600 text-sm">
                    {errors.password.message}
                  </p>
                )}
              </div>
            </div>

            <div className="mt-4">
              <div className="flex flex-row-reverse   ">
                <label className="flex items-center ml-6">
                  <input
                    type="radio"
                    value="admin"
                    {...register("accountType", {
                      required: "يرجى تحديد نوع الحساب",
                    })}
                    className="mr-2"
                  />
                  Admin
                </label>

                <label className="flex items-center mr-6">
                  <input
                    type="radio"
                    value="user"
                    {...register("accountType")}
                    className="mr-2"
                  />
                  User
                </label>
              </div>
              {errors.accountType && (
                <p className="text-red-600 text-sm">
                  {errors.accountType.message}
                </p>
              )}
            </div>
            <div>
              {error && (
                <p className="text-red-500 font-semibold mt-2">{error}</p>
              )}
            </div>
            <motion.button
              whileHover={{ scale: 1.05 }}
              whileTap={{ scale: 0.95 }}
              type="submit"
              className="w-full mt-4 bg-cyan-dark text-white py-2 rounded-lg hover:bg-cyan-900"
            >
              {isLoading ? (
                <Loader className="animate-spin mx-auto" size={24} />
              ) : (
                " إنشاء حساب"
              )}
            </motion.button>
            <p className="text-center mt-4">
              هل لديك حساب؟{" "}
              <Link to={"/login"} className="text-blue-600 ">
                تسجيل الدخول
              </Link>
            </p>
          </form>
        </div>
      </motion.div>
      <p className="text-center m-4">
        انشاء حساب كشركة؟{" "}
        <Link to={"/company-signup"} className="text-blue-600 ">
          انشاء حساب
        </Link>
      </p>
    </div>
  );
}
