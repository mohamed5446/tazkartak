import { useNavigate } from "react-router";
import { useAuthStore } from "../store/authStore";
import { motion } from "framer-motion";
import { useForm } from "react-hook-form";
import Cookies from "js-cookie";
import { Loader } from "lucide-react";
import axios from "axios";
export default function EmailVerification() {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm();

  const navigate = useNavigate();

  const { user, isLoading, verifyEmail, setdefaulte, role } = useAuthStore();

  const onSubmit = async (data) => {
    try {
      console.log(user);
      data.email = user;
      console.log(data);
      const response = await verifyEmail(data);
      const token = response.token;
      Cookies.set("token", token, { expires: 7, secure: true });
      if (role == "Admin") navigate("admin/profile");
      else if (role == "Company") navigate("company/profile");
      else navigate("/");
    } catch (error) {
      console.log(error);
    }
  };
  const cancle = async () => {
    await setdefaulte();
    navigate("/");
  };
  const ReSendCode = async () => {
    const data = { email: user };
    try {
      await axios.post(
        "https://tazkartk-api.runasp.net/api/Account/Send-OTP",
        data
      );
      console.log("successfull");
    } catch (error) {
      console.log(error);
    }
  };
  return (
    <motion.div
      initial={{ opacity: 0, y: -50 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.5 }}
      className="size-full flex justify-center items-center"
    >
      <div className="bg-white p-6 w-sm md:w-xl rounded-lg shadow-lg  h-fit text-center ">
        <h2 className="text-xl font-semibold text-gray-800">أدخل رمز الأمان</h2>
        <p className="text-gray-600 my-2"></p>
        <p className="text-gray-800 font-bold">
          لقد أرسلنا الكود الخاص بك إلى الايميل
        </p>
        <form onSubmit={handleSubmit(onSubmit)}>
          <input
            {...register("enteredOtp", {
              required: "يرجى إدخال رمز الامان",
              minLength: {
                value: 6,
                message: "must be 6 numbers",
              },
            })}
            type="text"
            placeholder="أدخل الرمز"
            className="w-full p-2 border border-gray-300 rounded mt-3 text-center"
          />
          {errors.enteredOtp && (
            <p className="text-red-500 text-end">{errors.enteredOtp.message}</p>
          )}
          <div className="flex justify-between">
            <div>
              <motion.button
                whileHover={{ scale: 1.1 }}
                whileTap={{ scale: 0.95 }}
                className="bg-gray-300 text-gray-800 px-4 py-2 rounded cursor-pointer"
                onClick={cancle}
                type="button"
              >
                إلغاء
              </motion.button>
              <motion.button
                whileHover={{ scale: 1.1 }}
                whileTap={{ scale: 0.95 }}
                className="bg-cyan-dark text-white px-4 py-2 rounded m-2 "
                type="submit"
              >
                {isLoading ? (
                  <Loader className="animate-spin mx-auto" size={24} />
                ) : (
                  " استمر"
                )}
              </motion.button>
            </div>

            <p
              onClick={ReSendCode}
              className="text-cyan-dark mt-4 cursor-pointer"
            >
              لم أحصل على الكود؟
            </p>
          </div>
        </form>
      </div>
    </motion.div>
  );
}
