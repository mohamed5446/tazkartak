import axios from "axios";
import { useEffect, useReducer, useState } from "react";
import { useForm } from "react-hook-form";
import Modal from "react-modal";
import { motion } from "framer-motion";
import { Eye, EyeOff, Loader } from "lucide-react";
import { Bounce, ToastContainer, toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import { useNavigate } from "react-router";

const customStyles = {
  content: {
    top: "50%",
    left: "50%",
    right: "auto",
    bottom: "auto",
    padding: "0px",
    width: "50%",
    maxWidth: "45rem",
    transform: "translate(-50%, -50%)",
    borderRadius: "25px",
  },
  overlay: {
    backgroundColor: "rgba(189, 189, 189, 0.1)",
  },
};
export default function Users() {
  const [users, setusers] = useState([]);
  const [user, setuser] = useState({});
  const [modalIsOpen, setIsOpen] = useState(false);
  const [modal2IsOpen, set2IsOpen] = useState(false);
  const [type, setType] = useState("");

  const [isLoading, setisLoading] = useState(false);
  const [isDeleting, setisDeleting] = useState(false);
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);

  const [ignored, forceUpdate] = useReducer((x) => x + 1, 0);
  const navigate = useNavigate();

  const {
    register: register2,
    handleSubmit: handleSubmit2,
    formState: { errors: errors2 },
    setValue: setValue2,
    watch,
  } = useForm();
  const newPassword = watch("password");

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
    formState: { errors },
    setValue,
  } = useForm();
  const fetchUsers = async () => {
    try {
      const res = await axios.get("https://tazkartk-api.runasp.net/api/Users");
      console.log(res);
      setusers(res.data);
    } catch (error) {
      console.log(error);
    }
  };
  const addedSuccessfully = () =>
    toast.success("added successfully", {
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
  useEffect(() => {
    try {
      setisLoading(true);
      fetchUsers();
      setisLoading(false);
    } catch (error) {
      setisLoading(false);
      console.log(error);
    }
  }, [ignored]);
  function openModal(user) {
    setuser(user);
    console.log(user);
    setValue("firstName", user.firstName);
    setValue("lastName", user.lastName);
    setValue("phone", user.phoneNumber);
    setValue("city", user.city);
    setValue("street", user.street);

    setIsOpen(true);
  }
  function closeModal() {
    setIsOpen(false);
  }
  function openModal2(type) {
    setType(type);
    setValue2("firstName", "");
    setValue2("email", "");
    setValue2("phoneNumber", "");
    setValue2("lastName", "");
    setValue2("password", "");
    setValue2("confirmPassword", "");
    set2IsOpen(true);
  }

  function closeModal2() {
    set2IsOpen(false);
  }
  const onSubmit = async (data) => {
    console.log(data);
    const formData = new FormData();
    formData.append("firstName", data.firstName);
    formData.append("lastName", data.lastName);
    formData.append("Phone", data.phone);
    formData.append("Photo", data.logo[0]);
    for (var pair of formData.entries()) {
      console.log(pair[0] + ", " + pair[1]);
    }
    try {
      setisLoading(true);
      const response = await axios.put(
        `https://tazkartk-api.runasp.net/api/Users/${user.id}`,
        formData,
        {
          headers: {
            "Content-Type": "multipart/form-data",
          },
        }
      );
      closeModal();
      console.log(response);
      forceUpdate();
      setisLoading(false);
      editeSuccessfull();
    } catch (error) {
      toast.error(error.response.data.message);
      setisLoading(false);
      console.log(error);
    }
  };
  const deleteCompany = async () => {
    console.log(user);
    try {
      setisDeleting(true);
      const response = await axios.delete(
        `https://tazkartk-api.runasp.net/api/Users/${user.id}`
      );
      closeModal();
      toast.success(response.data.message);
      console.log(response);
      setisDeleting(false);
      forceUpdate();
    } catch (error) {
      toast.error(error.response.data.message);
      setisDeleting(false);
      console.log(error);
    }
  };
  const onSubmit2 = async (data) => {
    console.log(data);

    try {
      setisLoading(true);
      if (type == "user") {
        const response = await axios.post(
          "https://tazkartk-api.runasp.net/api/Users",
          data
        );
        console.log(response);
      } else {
        const response = await axios.post(
          "https://tazkartk-api.runasp.net/api/Users/Add-Admin",
          data
        );
        console.log(response);
      }

      addedSuccessfully();
      closeModal2();

      forceUpdate();
      setisLoading(false);
    } catch (error) {
      setisLoading(false);
      toast.error(error.response.data.message);
      console.log(error);
    }
  };
  return (
    <div className="flex flex-col m-4 items-end  gap-4 p-2 w-full xl:w-2/4">
      <div className="flex items-center  justify-between w-full gap-2  ">
        <div className="flex flex-col md:flex-row">
          <button
            type="button"
            onClick={() => openModal2("user")}
            className="bg-cyan-dark mr-2 text-white p-2 rounded shadow-lg hover:cursor-pointer"
          >
            اضافة مستخدم
          </button>
          <button
            type="button"
            onClick={() => openModal2("admin")}
            className="bg-cyan-dark text-white p-2 rounded shadow-lg hover:cursor-pointer"
          >
            اضافة ادمن
          </button>
        </div>

        <p className="text-3xl text-cyan-dark font-bold ">المستخدمين</p>
      </div>
      {users.map((user) => (
        <div
          key={user.id}
          className="flex  bg-white p-2 rounded-lg shadow-lg w-full justify-between items-center"
        >
          <div>
            <button
              onClick={() => openModal(user)}
              className="bg-cyan-dark text-white p-2 px-6 m-2 rounded hover:cursor-pointer"
            >
              تعديل
            </button>
            <button
              onClick={() => navigate(`/admin/user/${user.id}`)}
              className="bg-cyan-dark text-white p-2 px-6 rounded hover:cursor-pointer"
            >
              عرض التذاكر
            </button>
          </div>

          <div className="flex flex-col-reverse md:flex-row gap-2 items-center p-2">
            <h3 className="text-lg font-bold mt-2 text-center">
              {user.firstName} {user.lastName}
            </h3>
            <img
              src={user.photoUrl}
              alt="user image"
              className="rounded-lg  size-16 xl:size-28"
            />
          </div>
        </div>
      ))}

      <Modal
        isOpen={modal2IsOpen}
        onRequestClose={closeModal2}
        style={customStyles}
      >
        <div className="bg-white opacity-100 p-6 rounded-lg shadow-lg w-full  text-black   space-y-4">
          <h2 className="text-3xl font-bold text-center">
            اضافة {type == "user" ? "مستخدم" : "ادمن"}
          </h2>

          {/* <img
            src={
              companylogo ||
              "https://cdn.pixabay.com/photo/2015/10/05/22/37/blank-profile-picture-973460_1280.png"
            }
            alt=""
            className="rounded place-self-center size-36"
          /> */}
          <form
            onSubmit={handleSubmit2(onSubmit2)}
            className="flex flex-col gap-2 "
          >
            <label className="text-end">
              الاسم الاول
              <input
                {...register2("firstName", {
                  required: "الاسم الاول مطلوب",
                  minLength: {
                    value: 3,
                    message: "يجب أن يكون الاسم 3 أحرف على الأقل",
                  },
                })}
                className="w-full border p-2 my-2 rounded"
              />
              {errors2.firstName && (
                <p className="text-red-500 text-sm">
                  {errors2.firstName.message}
                </p>
              )}
            </label>
            <label className="text-end">
              الاسم الثانى
              <input
                {...register2("lastName", {
                  required: "الاسم الاخير مطلوب",
                  minLength: {
                    value: 3,
                    message: "يجب أن يكون الاسم 3 أحرف على الأقل",
                  },
                })}
                className="w-full border p-2 my-2 rounded "
              />
              {errors2.lastName && (
                <p className="text-red-500 text-sm">
                  {errors2.lastName.message}
                </p>
              )}
            </label>

            <label className="text-end w-full">
              البريد الإلكتروني
              <input
                {...register2("email", {
                  required: "البريد الإلكتروني مطلوب",
                  pattern: {
                    value: /^\S+@\S+\.\S+$/,
                    message: "البريد الإلكتروني غير صالح",
                  },
                })}
                className="w-full border p-2 my-2 rounded "
              />
              {errors2.email && (
                <p className="text-red-500 text-sm">{errors2.email.message}</p>
              )}
            </label>

            <label className="text-end w-full">
              رقم الهاتف
              <input
                {...register2("phoneNumber", {
                  required: "رقم الهاتف مطلوب",
                  pattern: {
                    value: /^[0-9]{11}$/,
                    message: "يجب أن يكون رقم الهاتف 11 رقماً ",
                  },
                })}
                className="w-full border p-2 my-2 rounded "
              />
              {errors2.phoneNumber && (
                <p className="text-red-500 text-sm">
                  {errors2.phoneNumber.message}
                </p>
              )}
            </label>

            <label className="text-end">
              كلمة المرور
              <div className="relative mt-2">
                <input
                  type={showPassword ? "text" : "password"}
                  {...register2("password", {
                    required: "يرجى إدخال كلمة المرور",
                  })}
                  className="w-full border  p-2 rounded-lg focus:outline-none focus:ring-2 focus:ring-cyan-dark pr-10"
                />

                <button
                  type="button"
                  onClick={() => setShowPassword((prev) => !prev)}
                  className="absolute right-2 top-1/2 transform -translate-y-1/2 text-gray-600"
                >
                  {showPassword ? <Eye size={20} /> : <EyeOff size={20} />}
                </button>
              </div>
            </label>
            {errors2.password && (
              <p className="text-red-600 text-sm text-end">
                {errors2.password.message}
              </p>
            )}
            <div className="md:col-span-2 mt-2">
              <label className=" w-full block  mb-1 text-end">
                تأكيد كلمة السر
                <div className="relative mb-2">
                  <input
                    type={showConfirmPassword ? "text" : "password"}
                    {...register2("confirmPassword", {
                      required: "هذه الخانة مطلوبة",
                      validate: (value) =>
                        value === newPassword || "كلمة السر غير متطابقة",
                    })}
                    className="w-full border  p-2 rounded-lg focus:outline-none focus:ring-2 focus:ring-cyan-dark pr-10"
                  />

                  <button
                    type="button"
                    onClick={() => setShowConfirmPassword((prev) => !prev)}
                    className="absolute right-2 top-1/2 transform -translate-y-1/2 text-gray-600"
                  >
                    {showConfirmPassword ? (
                      <Eye size={20} />
                    ) : (
                      <EyeOff size={20} />
                    )}
                  </button>
                </div>
              </label>

              {errors2.confirmPassword && (
                <p className="text-red-500 text-sm text-end">
                  {errors2.confirmPassword.message}
                </p>
              )}
            </div>

            {/* <input
              {...register("logo")}
              className="bg-cyan-dark rounded text-white p-4"
              type="file"
              accept="image/*"
              onChange={(e) => {
                const file = e.target.files?.[0];
                setLogo(file ? URL.createObjectURL(file) : undefined);
              }}
            /> */}

            <div className="flex gap-2 ">
              <motion.button
                type="submit"
                disabled={isLoading}
                className="w-full bg-cyan-dark text-white py-2 rounded hover:bg-cyan-900"
              >
                {isLoading ? (
                  <Loader className="animate-spin mx-auto" size={24} />
                ) : (
                  "اضافة"
                )}
              </motion.button>

              <button
                type="button"
                onClick={closeModal2}
                className="w-full bg-red-500 text-white py-2 rounded hover:bg-cyan-900"
              >
                الغاء
              </button>
            </div>
          </form>
        </div>
      </Modal>
      <Modal
        isOpen={modalIsOpen}
        onRequestClose={closeModal}
        style={customStyles}
      >
        <div className="bg-white opacity-100 p-6 rounded-lg shadow-lg w-full  text-black   space-y-4">
          <h2 className="text-3xl font-bold text-center">تعديل البيانات</h2>

          <img
            src={user.photoUrl}
            alt=""
            className="rounded place-self-center size-36"
          />
          <form
            onSubmit={handleSubmit(onSubmit)}
            className="flex flex-col gap-5 "
          >
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
                className="w-full border p-2 my-2 rounded "
              />
              {errors.firstName && (
                <p className="text-red-500 text-sm">
                  {errors.firstName.message}
                </p>
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
                <p className="text-red-500 text-sm">
                  {errors.lastName.message}
                </p>
              )}
            </label>
            <label className="text-end">
              رقم الهاتف
              <input
                {...register("phone", {
                  required: "رقم الهاتف مطلوب",
                  pattern: {
                    value: /^[0-9]{11}$/,
                    message: "يجب أن يكون رقم الهاتف 11 رقماً ",
                  },
                })}
                className="w-full border p-2 my-2 rounded "
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
              <motion.button
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
              </motion.button>
            </div>
          </form>
        </div>
      </Modal>
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
