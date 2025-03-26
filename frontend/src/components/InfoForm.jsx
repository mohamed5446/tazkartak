import axios from "axios";
import { useEffect, useState } from "react";
import { useForm } from "react-hook-form";
import { useAuthStore } from "../store/authStore";
import { motion } from "framer-motion";
import { Loader } from "lucide-react";
export default function InfoForm() {
  const [user, setUser] = useState({});
  const { id } = useAuthStore();
  const [isLoading, setisLoading] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
    setValue,
  } = useForm();
  const onSubmit = (data) => {
    console.log(data);
  };
  const getUser = async () => {
    try {
      const res = await axios.get(
        `https://tazkartk-api.runasp.net/api/Users/${id}`
      );
      setUser(res.data);
      console.log(res.data);
      setValue("firstName", res.data.firstName);
      setValue("lastName", res.data.lastName);
      setValue("phone", res.data.phoneNumber);
      setValue("email", res.data.email);
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
            className="w-full border p-2 my-2 rounded text-end"
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
            className="w-full border p-2 my-2 rounded text-end"
          />
          {errors.lastName && (
            <p className="text-red-500 text-sm">{errors.lastName.message}</p>
          )}
        </label>
        <label className="text-end">
          رقم الهاتف
          <input
            {...register("phone", {
              required: "رقم الهاتف مطلوب",
              pattern: {
                value: /^[0-9]{10,11}$/,
                message: "يجب أن يكون رقم الهاتف من 10 إلى 11 رقماً",
              },
            })}
            className="w-full border p-2 my-2 rounded text-end"
          />
          {errors.phone && (
            <p className="text-red-500 text-sm">{errors.phone.message}</p>
          )}
        </label>
        {/* <hr className="border m-2 w-3/4 place-self-end" /> */}
        {/* <div>
              <h3 className="text-xl text-end mb-2 text-cyan-dark ">العنوان</h3>
              <div className="flex gap-1.5">
                <label className="w-full text-end">
                  المدينة
                  <select
                    {...register("city", {
                      required: "يجب اختيار المدينة",
                    })}
                    className="w-full border p-2 my-2 rounded text-end"
                  >
                    <option value="">اختر المدينة</option>
                    <option value="المعادي">المعادي</option>
                    <option value="string">المعادي</option>
                    <option value="الزمالك">الزمالك</option>
                  </select>
                  {errors.city && (
                    <p className="text-red-500 text-sm">
                      {errors.city.message}
                    </p>
                  )}
                </label>
              </div>
            </div>

            <label className="text-end">
              الشارع
              <input
                {...register("street", {
                  required: "يجب إدخال اسم الشارع",
                  minLength: {
                    value: 3,
                    message: "يجب أن يكون اسم الشارع 3 أحرف على الأقل",
                  },
                })}
                className="w-full border p-2 my-2 rounded"
              />
              {errors.street && (
                <p className="text-red-500 text-sm">{errors.street.message}</p>
              )}
            </label> */}

        <input
          {...register("logo")}
          className="bg-cyan-dark rounded text-white p-4"
          type="file"
          name="logo"
          // onChange={(e) => {
          //   const file = e.target.files?.[0];
          //   setLogo(file ? URL.createObjectURL(file) : undefined);
          // }}
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
          {/* <motion.button
                type="button"
                onClick={deleteCompany}
                disabled={isDeleting}
                className="w-full bg-red-500 text-white py-2 rounded hover:bg-red-800"
              >
                {isDeleting ? (
                  <Loader className="animate-spin mx-auto" size={24} />
                ) : (
                  "حذف"
                )}
              </motion.button> */}
        </div>
      </form>
    </div>
    // <div className="bg-white p-6 rounded-lg shadow-lg w-full max-w-2xl space-y-4 ">
    //   <h2 className="text-3xl font-bold text-end">حسابي</h2>
    //   <h3 className="text-xl text-end text-cyan-dark">البيانات الشخصية</h3>
    //   <form onSubmit={handleSubmit(onSubmit)} className="flex flex-col gap-5 ">
    //     <div className="flex gap-4 flex-row-reverse">
    //       <label className="text-end w-full">
    //         الاسم الاول
    //         <input
    //           {...register("firstName", {
    //             required: "الاسم الاول مطلوب",
    //             minLength: {
    //               value: 3,
    //               message: "يجب أن يكون الاسم 3 أحرف على الأقل",
    //             },
    //           })}
    //           className="w-full border p-2 my-2 rounded text-end"
    //         />
    //         {errors.firstName && (
    //           <p className="text-red-500 text-sm">{errors.firstName.message}</p>
    //         )}
    //       </label>
    //       <label className="text-end w-full">
    //         الاسم الثانى
    //         <input
    //           {...register("lastName", {
    //             required: "الاسم الثانى مطلوب",
    //             minLength: {
    //               value: 3,
    //               message: "يجب أن يكون الاسم 3 أحرف على الأقل",
    //             },
    //           })}
    //           className="w-full border p-2 my-2 rounded text-end"
    //         />
    //         {errors.lastName && (
    //           <p className="text-red-500 text-sm">{errors.lastName.message}</p>
    //         )}
    //       </label>
    //     </div>

    //     <label className="text-end">
    //       البريد الإلكتروني
    //       <input
    //         {...register("email", {
    //           required: "البريد الإلكتروني مطلوب",
    //           pattern: {
    //             value: /^\S+@\S+\.\S+$/,
    //             message: "البريد الإلكتروني غير صالح",
    //           },
    //         })}
    //         className="w-full border p-2 my-2 rounded text-end"
    //       />
    //       {errors.email && (
    //         <p className="text-red-500 text-sm">{errors.email.message}</p>
    //       )}
    //     </label>
    //     <label className="text-end">
    //       رقم الهاتف
    //       <input
    //         {...register("phone", {
    //           required: "رقم الهاتف مطلوب",
    //           pattern: {
    //             value: /^[0-9]{10,11}$/,
    //             message: "يجب أن يكون رقم الهاتف من 10 إلى 11 رقماً",
    //           },
    //         })}
    //         className="w-full border p-2 my-2 rounded text-end"
    //       />
    //       {errors.phone && (
    //         <p className="text-red-500 text-sm">{errors.phone.message}</p>
    //       )}
    //     </label>
    //     <hr className="border m-2 w-3/4 place-self-end" />
    //     <div>
    //       <h3 className="text-xl text-end mb-2 text-cyan-dark ">العنوان</h3>
    //       <div className="flex gap-1.5">
    //         <label className="w-full text-end">
    //           المنطقة
    //           <select
    //             {...register("region", { required: "يجب اختيار المنطقة" })}
    //             className="w-full border p-2 my-2 rounded text-end"
    //           >
    //             <option value="">اختر المنطقة</option>
    //             <option value="القاهرة">القاهرة</option>
    //             <option value="الإسكندرية">الإسكندرية</option>
    //           </select>
    //           {errors.region && (
    //             <p className="text-red-500 text-sm">{errors.region.message}</p>
    //           )}
    //         </label>
    //         <label className="w-full text-end">
    //           المدينة
    //           <select
    //             {...register("city", { required: "يجب اختيار المدينة" })}
    //             className="w-full border p-2 my-2 rounded text-end"
    //           >
    //             <option value="">اختر المدينة</option>
    //             <option value="المعادي">المعادي</option>
    //             <option value="الزمالك">الزمالك</option>
    //           </select>
    //           {errors.city && (
    //             <p className="text-red-500 text-sm">{errors.city.message}</p>
    //           )}
    //         </label>
    //       </div>
    //     </div>

    //     <label className="text-end">
    //       الشارع
    //       <input
    //         {...register("street", {
    //           required: "يجب إدخال اسم الشارع",
    //           minLength: {
    //             value: 3,
    //             message: "يجب أن يكون اسم الشارع 3 أحرف على الأقل",
    //           },
    //         })}
    //         className="w-full border p-2 my-2 rounded"
    //       />
    //       {errors.street && (
    //         <p className="text-red-500 text-sm">{errors.street.message}</p>
    //       )}
    //     </label>

    //     <button
    //       type="submit"
    //       className="w-full bg-cyan-dark text-white py-2 rounded hover:bg-cyan-900"
    //     >
    //       حفظ
    //     </button>
    //   </form>
    // </div>
  );
}
