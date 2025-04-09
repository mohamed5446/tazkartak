import axios from "axios";
import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { useAuthStore } from "../../store/authStore";
import { motion } from "framer-motion";
import { Loader } from "lucide-react";
import { Bounce, toast, ToastContainer } from "react-toastify";
export default function InfoForm() {
  const { id, User: user, fetchCompany } = useAuthStore();
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
    formData.append("Name", data.Name);
    formData.append("City", data.City);
    formData.append("Street", data.Street);
    formData.append("Phone", data.Phone);
    formData.append("Logo", data.logo[0]);
    console.log(data.logo);
    for (var pair of formData.entries()) {
      console.log(pair[0] + ", " + pair[1]);
    }
    try {
      setisLoading(true);
      const response = await axios.put(
        `https://tazkartk-api.runasp.net/api/Companies/${id}`,
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
      await fetchCompany(id);
    } catch (error) {
      setisLoading(false);
      toast.error("somthing went wrong! please try again.");
      console.log(error);
    }
  };

  const getUser = () => {
    try {
      setValue("Name", user.name);
      setValue("Phone", user.phone);
      setValue("City", user.city);
      setValue("Street", user.street);
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
        src={user.logo}
        alt=""
        className="rounded place-self-center size-36"
      />
      <form onSubmit={handleSubmit(onSubmit)} className="flex flex-col gap-5 ">
        <label className="text-end">
          <p>الاسم</p>

          <input
            {...register("Name", {
              required: "الاسم مطلوب",
              minLength: {
                value: 3,
                message: "يجب أن يكون الاسم 3 أحرف على الأقل",
              },
            })}
            className="w-full border p-2 my-2 rounded text-end"
          />
          {errors.name && (
            <p className="text-red-500 text-sm">{errors.name.message}</p>
          )}
        </label>

        <label className="text-end">
          رقم الهاتف
          <input
            {...register("Phone", {
              pattern: {
                value: /^[0-9]{10,11}$/,
                message: "يجب أن يكون رقم الهاتف من 10 إلى 11 رقماً",
              },
            })}
            className="w-full border p-2 my-2 rounded text-end"
          />
          {errors.phone && (
            <p className="text-red-500 text-sm">{errors.Phone.message}</p>
          )}
        </label>
        <div className="flex gap-2 ">
          <label className="text-end w-full">
            المدينة
            <input
              {...register("City")}
              className="w-full border p-2 my-2 rounded text-end"
            />
            {errors.City && (
              <p className="text-red-500 text-sm">{errors.City.message}</p>
            )}
          </label>
          {/* الشارع */}
          <label className="text-end w-full">
            الشارع
            <input
              {...register("Street")}
              className="w-full border p-2 my-2 rounded text-end"
            />
            {errors.Street && (
              <p className="text-red-500 text-sm">{errors.Street.message}</p>
            )}
          </label>
        </div>
        <input
          {...register("logo")}
          className="bg-cyan-dark rounded text-white p-4"
          type="file"
          name="logo"
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
