import { motion } from "framer-motion";
import { useForm } from "react-hook-form";
import { Link, useNavigate } from "react-router";
import { Loader } from "lucide-react";
import { useEffect } from "react";
import { useAuthStore } from "../../store/authStore";
export default function CompanySignUpPage() {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm();

  const { companySignup, error, isLoading, setdefaulte } = useAuthStore();

  const navigate = useNavigate();

  const onSubmit = async (data) => {
    console.log(data);
    try {
      await companySignup(data);
      navigate("/verify-email");
    } catch (error) {
      console.log(error.response);
    }
  };
  useEffect(() => {
    setdefaulte();
  }, []);
  return (
    <div className="size-full flex">
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
        <div className="w-2/3 mx-auto md:max-w-xl xl:max-w-4xl bg-white p-8 shadow-2xl rounded-lg text-end m-4">
          <h2 className="text-2xl text-cyan-dark font-bold mb-6 text-center">
            إنشاء حساب
          </h2>
          <form onSubmit={handleSubmit(onSubmit)}>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label className="block text-gray-700 mb-2">الاسم </label>
                <input
                  type="text"
                  {...register("name", {
                    required: "يرجى إدخال الاسم ",
                  })}
                  className="w-full border border-gray-300 p-2 rounded-lg focus:outline-none focus:ring-2 focus:ring-teal-700"
                />
                {errors.name && (
                  <p className="text-red-600 text-sm">{errors.name.message}</p>
                )}
              </div>

              <div>
                <label className="block text-gray-700 mb-2">رقم الهاتف</label>
                <input
                  type="text"
                  {...register("phone", {
                    required: "يرجى إدخال رقم الهاتف",
                  })}
                  className="w-full border border-gray-300 p-2 rounded-lg focus:outline-none focus:ring-2 focus:ring-teal-700"
                />
                {errors.phone && (
                  <p className="text-red-600 text-sm">{errors.phone.message}</p>
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

              <div>
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
              <div>
                <label className="block text-gray-700 mb-2">المدينة</label>
                <input
                  type="text"
                  {...register("city", {
                    required: "يرجى إدخال المدينة",
                  })}
                  className="w-full border border-gray-300 p-2 rounded-lg focus:outline-none focus:ring-2 focus:ring-teal-700"
                />
                {errors.city && (
                  <p className="text-red-600 text-sm">{errors.city.message}</p>
                )}
              </div>
              <div>
                <label className="block text-gray-700 mb-2">الشارع</label>
                <input
                  type="street"
                  {...register("street", {
                    required: "يرجى إدخال الشارع",
                  })}
                  className="w-full border border-gray-300 p-2 rounded-lg focus:outline-none focus:ring-2 focus:ring-teal-700"
                />
                {errors.street && (
                  <p className="text-red-600 text-sm">
                    {errors.street.message}
                  </p>
                )}
              </div>

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
    </div>
  );
}
