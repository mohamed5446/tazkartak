import axios from "axios";
import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { useAuthStore } from "../../store/authStore";
import { motion } from "framer-motion";
import { Loader } from "lucide-react";
import { Bounce, toast, ToastContainer } from "react-toastify";
export default function InfoForm() {
  const { id, User: user, fetchUser } = useAuthStore();
  const [isLoading, setisLoading] = useState(false);
  const editeSuccessfull = () =>
    toast.success("edited successfully", {
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
  const {
    register,
    handleSubmit,
    setValue,
    formState: { errors },
  } = useForm();
  const onSubmit = async (data) => {
    console.log(data);
    const formData = new FormData();
    formData.append("firstName", data.firstName);
    formData.append("lastName", data.lastName);
    formData.append("Phone", data.Phone);
    formData.append("photo", data.photo[0]);
    for (var pair of formData.entries()) {
      console.log(pair[0] + ", " + pair[1]);
    }
    try {
      setisLoading(true);
      const response = await axios.put(
        `https://tazkartk-api.runasp.net/api/Users/${id}`,
        formData,
        {
          headers: {
            "Content-Type": "multipart/form-data",
          },
        }
      );

      console.log(response);
      setisLoading(false);
      editeSuccessfull();
      await fetchUser(id);
    } catch (error) {
      setisLoading(false);
      toast.error("somthing went wrong! please try again.");
      console.log(error);
    }
  };

  const getUser = () => {
    try {
      setValue("firstName", user.firstName);
      setValue("lastName", user.lastName);
      setValue("Phone", user.phoneNumber);
    } catch (error) {
      console.log(error);
    }
  };
  useEffect(() => {
    getUser();
  }, []);
  return (
    <div className="bg-white p-6 rounded-lg shadow-lg w-full max-w-2xl space-y-4">
      <h2 className="text-3xl font-bold text-center">تعديل البيانات</h2>

      <img
        src={user.photoUrl}
        alt=""
        className="rounded place-self-center size-36"
      />
      <form onSubmit={handleSubmit(onSubmit)} className="flex flex-col gap-5 ">
        <label className="text-end">
          <p>الاسم الاول</p>

          <input
            {...register("firstName", {
              required: "الاسم مطلوب",
              minLength: {
                value: 3,
                message: "يجب أن يكون الاسم 3 أحرف على الأقل",
              },
            })}
            className="w-full border p-2 my-2 rounded"
          />
          {errors.firstName && (
            <p className="text-red-500 text-sm">{errors.firstName.message}</p>
          )}
        </label>
        <div className="grid grid-cols-2 gap-2"></div>
        <label className="text-end">
          <p>اسم العائلة</p>

          <input
            {...register("lastName", {
              required: "الاسم مطلوب",
              minLength: {
                value: 3,
                message: "يجب أن يكون الاسم 3 أحرف على الأقل",
              },
            })}
            className="w-full border p-2 my-2 rounded "
          />
          {errors.lastName && (
            <p className="text-red-500 text-sm">{errors.lastName.message}</p>
          )}
        </label>
        <label className="text-end">
          رقم الهاتف
          <input
            {...register("Phone", {
              required: "رقم الهاتف مطلوب",
              pattern: {
                value: /^[0-9]{10,11}$/,
                message: "يجب أن يكون رقم الهاتف من 10 إلى 11 رقماً",
              },
            })}
            className="w-full border p-2 my-2 rounded "
          />
          {errors.phone && (
            <p className="text-red-500 text-sm">{errors.phone.message}</p>
          )}
        </label>

        <input
          {...register("photo")}
          className="bg-cyan-dark rounded text-white p-4"
          type="file"
          name="photo"
        />

        <div className="flex gap-2 ">
          <motion.button
            type="submit"
            disabled={isLoading}
            className="w-full bg-cyan-dark text-white py-2 rounded hover:bg-cyan-900"
          >
            {isLoading ? (
              <Loader className="animate-spin mx-auto" size={24} />
            ) : (
              "تعديل"
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
