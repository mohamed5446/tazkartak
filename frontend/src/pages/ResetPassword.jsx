import { useForm } from "react-hook-form";
import { useAuthStore } from "../store/authStore";
import { motion } from "framer-motion";
import { useState } from "react";
import { Eye, EyeOff, Loader } from "lucide-react";
import { Bounce, toast, ToastContainer } from "react-toastify";
import { useNavigate } from "react-router";
import Cookies from "js-cookie";
import axios from "axios";

export default function ResetPassword() {
  const [isLoading, setisLoading] = useState(false);
  const [showNewPassword, setShowNewPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  const { user: email, resetPassword } = useAuthStore();
  const [isSendingOtp, setisSendingOtp] = useState(false);
  const navigate = useNavigate();

  const passwordChangedfailed = (error) => {
    toast.error(error, {
      position: "top-right",
      autoClose: 5000,
      hideProgressBar: false,
      closeOnClick: false,
      pauseOnHover: true,
      draggable: true,
      progress: undefined,
      theme: "light",
      transition: Bounce,
    });
  };
  const {
    register,
    handleSubmit,
    watch,
    formState: { errors },
  } = useForm();
  const newPassword = watch("newPassword");
  const changePassword = async (newData) => {
    console.log(newData);
    try {
      setisLoading(true);

      const response = await resetPassword(newData);
      setisLoading(false);
      console.log(response.roles[0]);
      const token = response.token;
      Cookies.set("token", token, { expires: 7, secure: true });
      if (response.roles[0] == "Admin") navigate("/admin/profile");
      else if (response.roles[0] == "Company") navigate("/company/profile");
      else navigate("/");
    } catch (error) {
      setisLoading(false);
      passwordChangedfailed(error.response.data.message);
      console.log(error);
    }
  };
  const onSubmit = (data) => {
    console.log(data, email);
    const newData = {
      email,
      otp: data.enteredOtp,
      newPasswod: data.newPassword,
    };
    console.log(newData);
    changePassword(newData);
  };
  const resendPassword = async () => {
    const data = { email: email };
    try {
      setisSendingOtp(true);
      console.log(email);
      const res = await axios.post(
        "https://tazkartk-api.runasp.net/api/Account/Forget-Password",
        data
      );
      console.log(res);
      setisSendingOtp(false);
      toast.success(res.data.message);
    } catch (error) {
      setisSendingOtp(false);
      toast.error(error.response.data.message);
      console.log(error);
    }
  };
  return (
    <div className="bg-white shadow-lg rounded-lg p-6 w-full flex flex-col max-w-2xl text-end m-auto mt-16">
      <h2 className="text-xl font-semibold text-gray-700 text-center mb-4">
        تغيير كلمة السر
      </h2>
      <form onSubmit={handleSubmit(onSubmit)}>
        <div className="mb-4">
          <label className="block text-gray-600 mb-1 ">رمز الأمان</label>
          <input
            {...register("enteredOtp", {
              required: "يرجى إدخال رمز الامان",
              minLength: {
                value: 6,
                message: "must be 6 numbers",
              },
            })}
            type="text"
            className="w-full p-2 border border-gray-300 rounded"
          />
          {errors.enteredOtp && (
            <p className="text-red-500 text-sm text-end">
              {errors.enteredOtp.message}
            </p>
          )}
        </div>

        <div className="mb-4">
          <label className="block text-gray-600 mb-1">كلمة السر الجديدة</label>

          <div className="relative">
            <input
              type={showNewPassword ? "text" : "password"}
              {...register("newPassword", {
                required: " يرجى إدخال كلمة المرورالجديدة",
                pattern: {
                  value: /^(?=.*\d)(?=.*[A-Z])(?=.*\W).+$/, // Requires at least one digit and one uppercase letter
                  message:
                    " وحرف واحد على الاقل غير ابجدى(A-Z)يجب أن تحتوي كلمة المرور على رقم واحد (0-9) وحرف كبير واحد  على الأقل",
                },
              })}
              className="w-full border border-gray-300 p-2 rounded-lg focus:outline-none focus:ring-2 focus:ring-cyan-dark pr-10"
            />

            <button
              type="button"
              onClick={() => setShowNewPassword((prev) => !prev)}
              className="absolute right-2 top-1/2 transform -translate-y-1/2 text-gray-600"
            >
              {showNewPassword ? <Eye size={20} /> : <EyeOff size={20} />}
            </button>
          </div>
          {errors.newPassword && (
            <p className="text-red-500 text-sm">{errors.newPassword.message}</p>
          )}
        </div>

        <div className="mb-4">
          <label className="block text-gray-600 mb-1">
            تأكيد كلمة السر الجديدة
          </label>

          <div className="relative">
            <input
              type={showConfirmPassword ? "text" : "password"}
              {...register("confirmPassword", {
                required: "يرجى تاكيد كلمة المرور",
                validate: (value) =>
                  value === newPassword || "كلمة السر غير متطابقة",
              })}
              className="w-full border border-gray-300 p-2 rounded-lg focus:outline-none focus:ring-2 focus:ring-cyan-dark pr-10"
            />

            <button
              type="button"
              onClick={() => setShowConfirmPassword((prev) => !prev)}
              className="absolute right-2 top-1/2 transform -translate-y-1/2 text-gray-600"
            >
              {showConfirmPassword ? <Eye size={20} /> : <EyeOff size={20} />}
            </button>
          </div>
          {errors.confirmPassword && (
            <p className="text-red-500 text-sm">
              {errors.confirmPassword.message}
            </p>
          )}
        </div>

        <div className="flex flex-row justify-between items-center">
          {isSendingOtp ? (
            <Loader className="animate-spin" size={24} />
          ) : (
            <p
              onClick={resendPassword}
              className="text-blue-500 hover:cursor-pointer"
            >
              إعادة إرسال رمز الأمان
            </p>
          )}

          <motion.button
            whileHover={{ scale: 1.1 }}
            whileTap={{ scale: 0.95 }}
            className="px-4 bg-cyan-dark text-white py-2 rounded-lg hover:bg-cyan-900 transition"
            type="submit"
            disabled={isLoading}
          >
            {isLoading ? (
              <Loader
                className="animate-spin mx-auto place-self-start"
                size={24}
              />
            ) : (
              "حفظ"
            )}
          </motion.button>
        </div>
      </form>
      <ToastContainer
        position="top-right"
        autoClose={5000}
        hideProgressBar={false}
        newestOnTop={false}
        closeOnClick={false}
        rtl={false}
        pauseOnFocusLoss
        draggable
        pauseOnHover
        theme="light"
        transition={Bounce}
      />
    </div>
  );
}
