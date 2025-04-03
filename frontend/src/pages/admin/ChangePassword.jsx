import axios from "axios";
import { useForm } from "react-hook-form";
import { useAuthStore } from "../../store/authStore";
import { motion } from "framer-motion";
import { useState } from "react";
import { Loader } from "lucide-react";
import { Bounce, toast, ToastContainer } from "react-toastify";

export default function ChangePassword() {
  const [isLoading, setisLoading] = useState(false);
  const passwordChangedSuccessfully = () =>
    toast.success("password changed successfully", {
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
  const { user: email } = useAuthStore();
  const newPassword = watch("newPassword");
  const changePassword = async (newData) => {
    try {
      setisLoading(true);
      const res = await axios.put(
        "https://tazkartk-api.runasp.net/api/Account/Change-Password",
        newData
      );
      console.log(res);
      setisLoading(false);
      passwordChangedSuccessfully();
    } catch (error) {
      setisLoading(false);
      passwordChangedfailed(error.response.data.message);
      console.log(error);
    }
  };
  const onSubmit = (data) => {
    const newData = {
      email,
      oldPassword: data.oldPassword,
      newPassword: data.newPassword,
    };
    console.log(newData);
    changePassword(newData);
  };
  return (
    <div className="bg-white shadow-lg rounded-lg p-6 w-full flex flex-col max-w-2xl text-end">
      <h2 className="text-xl font-semibold text-gray-700 text-center mb-4">
        تغيير كلمة السر
      </h2>
      <form onSubmit={handleSubmit(onSubmit)}>
        <div className="mb-4">
          <label className="block text-gray-600 mb-1">كلمة السر القديمة</label>
          <input
            type="password"
            {...register("oldPassword", { required: "هذه الخانة مطلوبة" })}
            className="w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring "
          />
          {errors.oldPassword && (
            <p className="text-red-500 text-sm">{errors.oldPassword.message}</p>
          )}
        </div>

        <div className="mb-4">
          <label className="block text-gray-600 mb-1">كلمة السر الجديدة</label>
          <input
            type="password"
            {...register("newPassword", {
              required: "هذه الخانة مطلوبة",
              pattern: {
                value: /^(?=.*\d)(?=.*[A-Z])(?=.*\W).+$/, // Requires at least one digit and one uppercase letter
                message:
                  " وحرف واحد على الاقل غير ابجدى(A-Z)يجب أن تحتوي كلمة المرور على رقم واحد (0-9) وحرف كبير واحد  على الأقل",
              },
            })}
            className="w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring "
          />
          {errors.newPassword && (
            <p className="text-red-500 text-sm">{errors.newPassword.message}</p>
          )}
        </div>

        <div className="mb-4">
          <label className="block text-gray-600 mb-1">
            تأكيد كلمة السر الجديدة
          </label>
          <input
            type="password"
            {...register("confirmPassword", {
              required: "هذه الخانة مطلوبة",
              validate: (value) =>
                value === newPassword || "كلمة السر غير متطابقة",
            })}
            className="w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring "
          />
          {errors.confirmPassword && (
            <p className="text-red-500 text-sm">
              {errors.confirmPassword.message}
            </p>
          )}
        </div>
        <div className="flex justify-end">
          <motion.button
            whileHover={{ scale: 1.1 }}
            whileTap={{ scale: 0.95 }}
            className=" px-4 bg-cyan-dark text-white py-2 rounded-lg  hover:bg-cyan-900 transition"
            type="submit"
          >
            {isLoading ? (
              <Loader className="animate-spin mx-auto" size={24} />
            ) : (
              " حفظ"
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
