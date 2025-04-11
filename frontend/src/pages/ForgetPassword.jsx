import { motion } from "framer-motion";
import { useForm } from "react-hook-form";
import { useAuthStore } from "../store/authStore";
import { useNavigate } from "react-router";
import { Loader } from "lucide-react";
import { useState } from "react";
import axios from "axios";
import { Bounce, toast, ToastContainer } from "react-toastify";

export default function ForgetPassword() {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm();
  const navigate = useNavigate();
  const [isLoading, setisLoading] = useState(false);
  const { setdefaulte, setuser } = useAuthStore();
  const onSubmit = async (data) => {
    try {
      setisLoading(true);
      const res = await axios.post(
        "https://tazkartk-api.runasp.net/api/Account/Forget-Password",
        data
      );
      setuser(res.data.email);

      navigate("/reset-password");
      setisLoading(false);
    } catch (error) {
      toast.error(error.response.data.message);
      setisLoading(false);
      console.log(error);
    }
  };
  const cancle = async () => {
    await setdefaulte();
    navigate("/");
  };

  return (
    <motion.div
      initial={{ opacity: 0, y: -50 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.5 }}
      className="size-full flex justify-center items-center"
    >
      <div className="bg-white p-6 w-sm md:w-xl rounded-lg shadow-lg  h-fit text-center ">
        <h2 className="text-xl font-semibold text-gray-800">
          أدخل الإيميل الخاص بك
        </h2>
        <p className="text-gray-600 my-2"></p>
        <p className="text-gray-800 font-bold">
          لكى نرسلنا الكود الخاص بك إلى الايميل
        </p>
        <form onSubmit={handleSubmit(onSubmit)}>
          <input
            {...register("email", {
              required: "يرجى إدخال الإيميل",
            })}
            type="text"
            className="w-full p-2 border border-gray-300 rounded mt-3 "
          />
          {errors.email && (
            <p className="text-red-500 text-end">{errors.email.message}</p>
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
          </div>
        </form>
      </div>
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
    </motion.div>
  );
}
