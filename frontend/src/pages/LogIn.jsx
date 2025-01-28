import { useForm } from "react-hook-form";
import { Link } from "react-router";

export default function LoginPage() {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm();

  const onSubmit = (data) => {
    console.log(data);
  };

  return (
    <div className="flex flex-col items-center">
      <p className="text-3xl font-bold p-4">
        <span className="text-blue-500">Taz</span>
        <span className="text-orange-400">kartk</span>
      </p>
      <div className="w-2/3 mx-auto max-w-xl bg-white p-8 shadow-2xl rounded-lg text-end">
        <form onSubmit={handleSubmit(onSubmit)} className="">
          <h2 className="text-2xl  font-bold mb-4 text-center">تسجيل الدخول</h2>

          <div className="mb-4">
            <label className="block text-gray-700 mb-2">رقم الهاتف</label>
            <input
              type="text"
              {...register("phone", { required: "يرجى إدخال رقم الهاتف" })}
              className="w-full border border-gray-300 p-2 rounded-lg focus:outline-none focus:ring-2 focus:ring-teal-700"
            />
            {errors.phone && (
              <p className="text-red-600 text-sm mt-1">
                {errors.phone.message}
              </p>
            )}
          </div>

          <div className="mb-4">
            <label className="block text-gray-700 mb-2">كلمة المرور</label>
            <input
              type="password"
              {...register("password", { required: "يرجى إدخال كلمة المرور" })}
              className="w-full border border-gray-300 p-2 rounded-lg focus:outline-none focus:ring-2 focus:ring-teal-700"
            />
            {errors.password && (
              <p className="text-red-600 text-sm mt-1">
                {errors.password.message}
              </p>
            )}
          </div>

          <div className="mb-4">
            <div className="flex justify-end space-x-4 rtl:space-x-reverse">
              <label className="flex items-center">
                <input
                  type="radio"
                  value="admin"
                  {...register("accountType", {
                    required: "يرجى تحديد نوع الحساب",
                  })}
                  className="mr-2"
                />
                Admin
              </label>
              <label className="flex items-center">
                <input
                  type="radio"
                  value="company"
                  {...register("accountType")}
                  className="mr-2"
                />
                Company
              </label>
              <label className="flex items-center">
                <input
                  type="radio"
                  value="user"
                  {...register("accountType")}
                  className="mr-2"
                />
                User
              </label>
            </div>
            {errors.accountType && (
              <p className="text-red-600 text-sm mt-1">
                {errors.accountType.message}
              </p>
            )}
          </div>

          <button
            type="submit"
            className="w-full bg-cyan-dark text-white py-2 rounded-lg hover:bg-cyan-900"
          >
            تسجيل الدخول
          </button>
        </form>
        <div className="flex flex-row-reverse pt-4">
          <p className="ml-2">ليس لديك حساب ؟ </p>
          <Link className="text-blue-600" to={"/signup"}>
            {" "}
            انشاء حساب
          </Link>
        </div>
      </div>
    </div>
  );
}
