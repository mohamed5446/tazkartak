import { useForm } from "react-hook-form";
import { useAuthStore } from "../../store/authStore";
import { Link, useNavigate } from "react-router";
import { motion } from "framer-motion";
import { useEffect } from "react";
export default function AdminProfile() {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm();
  const { logout } = useAuthStore();
  const onSubmit = (data) => {
    console.log(data);
  };
  const navigate = useNavigate();
  const signOut = async () => {
    await logout();
    navigate("/");
  };
  useEffect(() => {}, []);
  return (
    <motion.div
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.5 }}
      className="p-10 flex flex-col-reverse md:flex-row md:items-start items-end justify-center gap-10"
    >
      <div className="bg-white p-6 rounded-lg shadow-lg w-full max-w-2xl space-y-4">
        <h2 className="text-3xl font-bold text-end">حسابي</h2>
        <h3 className="text-xl text-end text-cyan-dark">البيانات الشخصية</h3>
        <form
          onSubmit={handleSubmit(onSubmit)}
          className="flex flex-col gap-5 "
        >
          <label className="text-end">
            الاسم
            <input
              {...register("name", {
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
            البريد الإلكتروني
            <input
              {...register("email", {
                required: "البريد الإلكتروني مطلوب",
                pattern: {
                  value: /^\S+@\S+\.\S+$/,
                  message: "البريد الإلكتروني غير صالح",
                },
              })}
              className="w-full border p-2 my-2 rounded text-end"
            />
            {errors.email && (
              <p className="text-red-500 text-sm">{errors.email.message}</p>
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
          <hr className="border m-2 w-3/4 place-self-end" />
          <div>
            <h3 className="text-xl text-end mb-2 text-cyan-dark ">العنوان</h3>
            <div className="flex gap-1.5">
              <label className="w-full text-end">
                المنطقة
                <select
                  {...register("region", { required: "يجب اختيار المنطقة" })}
                  className="w-full border p-2 my-2 rounded text-end"
                >
                  <option value="">اختر المنطقة</option>
                  <option value="القاهرة">القاهرة</option>
                  <option value="الإسكندرية">الإسكندرية</option>
                </select>
                {errors.region && (
                  <p className="text-red-500 text-sm">
                    {errors.region.message}
                  </p>
                )}
              </label>
              <label className="w-full text-end">
                المدينة
                <select
                  {...register("city", { required: "يجب اختيار المدينة" })}
                  className="w-full border p-2 my-2 rounded text-end"
                >
                  <option value="">اختر المدينة</option>
                  <option value="المعادي">المعادي</option>
                  <option value="الزمالك">الزمالك</option>
                </select>
                {errors.city && (
                  <p className="text-red-500 text-sm">{errors.city.message}</p>
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
          </label>

          <button
            type="submit"
            className="w-full bg-cyan-dark text-white py-2 rounded hover:bg-cyan-900"
          >
            حفظ
          </button>
        </form>
      </div>

      <div className="bg-gray-100  shadow-lg w-fit h-fit text-end">
        <div className="bg-cyan-dark p-10 rounded-lg rounded-b-none  text-white flex flex-row-reverse gap-2">
          <div className="bg-gray-400 p-14 rounded-full"></div>
          <div>
            <p className="text-lg font-bold m-1">أحمد صالح محمد الصغير</p>
            <p className="m-1">canadaeg13@gmail.com</p>
            <p className="m-1">01013069392</p>
          </div>
        </div>
        <div className="p-10 pt-0">
          <ul>
            <li className="mb-2 hover:text-cyan-dark font-bold cursor-pointer">
              <Link to={"/admin/profile"}>حسابى</Link>
            </li>

            <li className="mb-2 hover:text-cyan-dark cursor-pointer">
              <Link to={"/admin/companies"}>الشركات</Link>
            </li>
            <li className="mb-2 hover:text-cyan-dark cursor-pointer">
              <Link to={"/admin/users"}>المستخدمين</Link>
            </li>
            <li className="mb-2 hover:text-cyan-dark cursor-pointer">
              <Link to={"/change-password"}>تغيير كلمة السر</Link>
            </li>
            <li
              onClick={signOut}
              className="text-red-600 hover:text-red-800 cursor-pointer border-t-2 pt-4 border-dashed border-black "
            >
              تسجيل الخروج
            </li>
          </ul>
        </div>
      </div>
    </motion.div>
  );
}
