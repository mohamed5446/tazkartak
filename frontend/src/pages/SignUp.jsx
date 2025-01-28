import { useForm } from "react-hook-form";
import { Link } from "react-router";

export default function SignUpPage() {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm();

  const onSubmit = (data) => {
    console.log(data);
  };

  return (
    <div className="w-2/3 mx-auto max-w-xl bg-white p-8 shadow-2xl rounded-lg text-end m-4">
      <h2 className="text-2xl text-cyan-dark font-bold mb-6 text-center">
        إنشاء حساب
      </h2>
      <form onSubmit={handleSubmit(onSubmit)}>
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div>
            <label className="block text-gray-700 mb-2">اسم</label>
            <input
              type="text"
              {...register("name", { required: "يرجى إدخال الاسم" })}
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
              {...register("phone", { required: "يرجى إدخال رقم الهاتف" })}
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
              {...register("password", { required: "يرجى إدخال كلمة المرور" })}
              className="w-full border border-gray-300 p-2 rounded-lg focus:outline-none focus:ring-2 focus:ring-teal-700"
            />
            {errors.password && (
              <p className="text-red-600 text-sm">{errors.password.message}</p>
            )}
          </div>
        </div>

        <div className="mt-4">
          <div className="flex space-x-4 flex-row-reverse">
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
            <label className="flex items-center mx-2">
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
            <p className="text-red-600 text-sm">{errors.accountType.message}</p>
          )}
        </div>

        <button
          type="submit"
          className="w-full mt-6 bg-cyan-dark text-white py-2 rounded-lg hover:bg-cyan-900"
        >
          إنشاء حساب
        </button>
        <p className="text-center mt-4">
          هل لديك حساب؟{" "}
          <Link to={"/login"} className="text-blue-600 ">
            تسجيل الدخول
          </Link>
        </p>
      </form>
    </div>
  );
}
