import { motion } from "framer-motion";
import { useForm } from "react-hook-form";
import { Link, useNavigate } from "react-router";
import { useAuthStore } from "../store/authStore";
import { Eye, EyeOff, Loader } from "lucide-react";
import { useEffect, useState } from "react";
export default function SignUpPage() {
  const {
    register,
    handleSubmit,
    watch,
    formState: { errors },
  } = useForm();
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);

  const { userSignup, error, isLoading, setdefaulte } = useAuthStore();

  const navigate = useNavigate();
  const newPassword = watch("password");

  const onSubmit = async (data) => {
    try {
      await userSignup(data);
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
                  className="w-full border border-gray-300 p-2 rounded-lg focus:outline-none focus:ring-2 focus:ring-cyan-dark"
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
                  className="w-full border border-gray-300 p-2 rounded-lg focus:outline-none focus:ring-2 focus:ring-cyan-dark"
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
                  className="w-full border border-gray-300 p-2 rounded-lg focus:outline-none focus:ring-2 focus:ring-cyan-dark"
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
                  className="w-full border border-gray-300 p-2 rounded-lg focus:outline-none focus:ring-2 focus:ring-cyan-dark"
                />
                {errors.email && (
                  <p className="text-red-600 text-sm">{errors.email.message}</p>
                )}
              </div>

              <div className="md:col-span-2">
                <label className="block text-gray-700 mb-2">كلمة المرور</label>
                <div className="relative">
                  <input
                    type={showPassword ? "text" : "password"}
                    {...register("password", {
                      required: "Password is required",
                      pattern: {
                        value: /^(?=.*\d)(?=.*[A-Z])(?=.*\W).+$/,
                        message:
                          " وحرف واحد على الاقل غير ابجدى(A-Z)يجب أن تحتوي كلمة المرور على رقم واحد (0-9) وحرف كبير واحد  على الأقل",
                      },
                    })}
                    className="w-full border border-gray-300 p-2 rounded-lg focus:outline-none focus:ring-2 focus:ring-cyan-dark pr-10"
                  />

                  <button
                    type="button"
                    data-testid="toggle-password"
                    onClick={() => setShowPassword((prev) => !prev)}
                    className="absolute right-2 top-1/2 transform -translate-y-1/2 text-gray-600"
                  >
                    {showPassword ? <Eye size={20} /> : <EyeOff size={20} />}
                  </button>
                </div>
                {errors.password && (
                  <p className="text-red-600 text-sm">
                    {errors.password.message}
                  </p>
                )}
              </div>
              <div className="md:col-span-2">
                <label className=" w-full block text-gray-600 mb-1">
                  تأكيد كلمة السر
                </label>

                <div className="relative">
                  <input
                    type={showConfirmPassword ? "text" : "password"}
                    {...register("confirmPassword", {
                      required: "هذه الخانة مطلوبة",
                      validate: (value) =>
                        value === newPassword || "كلمة السر غير متطابقة",
                    })}
                    className="w-full border border-gray-300 p-2 rounded-lg focus:outline-none focus:ring-2 focus:ring-cyan-dark pr-10"
                  />

                  <button
                    data-testid="toggle-password"
                    type="button"
                    onClick={() => setShowConfirmPassword((prev) => !prev)}
                    className="absolute right-2 top-1/2 transform -translate-y-1/2 text-gray-600"
                  >
                    {showConfirmPassword ? (
                      <Eye size={20} />
                    ) : (
                      <EyeOff size={20} />
                    )}
                  </button>
                </div>
                {errors.confirmPassword && (
                  <p className="text-red-500 text-sm">
                    {errors.confirmPassword.message}
                  </p>
                )}
              </div>
            </div>

            <div>
              {console.log(error)}
              {error && (
                <p className="text-red-500 font-semibold mt-2">
                  {error.message}
                </p>
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
    </div>
  );
}
