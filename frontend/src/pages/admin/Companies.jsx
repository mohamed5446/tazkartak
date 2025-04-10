import { useEffect, useReducer, useState } from "react";
import axios from "axios";
import Modal from "react-modal";
import { useForm } from "react-hook-form";
import { motion } from "framer-motion";
import { Eye, EyeOff, Loader } from "lucide-react";
import { Bounce, ToastContainer, toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import { useNavigate } from "react-router";
import Cookies from "js-cookie";

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
Modal.setAppElement("#root");

export default function Companies() {
  const [companies, setCompanies] = useState([]);
  const [companylogo, setLogo] = useState("");
  const [isLoading, setisLoading] = useState(false);
  const [isDeleting, setisDeleting] = useState(false);
  const [company, setCompany] = useState({});
  const [modalIsOpen, setIsOpen] = useState(false);
  const [modal2IsOpen, set2IsOpen] = useState(false);
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);

  const [ignored, forceUpdate] = useReducer((x) => x + 1, 0);
  const navigate = useNavigate();

  const fetchCompanies = async () => {
    try {
      const res = await axios.get(
        "https://tazkartk-api.runasp.net/api/Companies"
      );
      console.log(res);
      setCompanies(res.data);
    } catch (error) {
      console.log(error);
    }
  };
  const {
    register,
    handleSubmit,
    formState: { errors },
    setValue,
  } = useForm();
  const {
    register: register2,
    handleSubmit: handleSubmit2,
    formState: { errors: errors2 },
    setValue: setValue2,
    watch,
  } = useForm();
  const newPassword = watch("password");
  useEffect(() => {
    try {
      fetchCompanies();
    } catch (error) {
      console.log(error);
    }
  }, [ignored]);
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
  function openModal(company) {
    setCompany(company);
    console.log(company);
    setValue("name", company.name);
    setValue("email", company.email);
    setValue("phone", company.phone);
    setValue("city", company.city);
    setValue("street", company.street);

    setIsOpen(true);
  }

  function closeModal() {
    setLogo(undefined);
    setIsOpen(false);
  }
  function openModal2() {
    setValue2("name", "");
    setValue2("email", "");
    setValue2("phone", "");
    setValue2("city", "");
    setValue2("street", "");
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
    formData.append("Name", data.name);
    formData.append("City", data.city);
    formData.append("Street", data.street);
    formData.append("Logo", data.logo[0]);
    formData.append("PhoneNumber", data.phone);
    for (var pair of formData.entries()) {
      console.log(pair[0] + ", " + pair[1]);
    }
    try {
      setisLoading(true);
      const response = await axios.put(
        `https://tazkartk-api.runasp.net/api/Companies/${company.id}`,
        formData,
        {
          headers: {
            "Content-Type": "multipart/form-data",
          },
        }
      );
      editeSuccessfull();
      closeModal();
      console.log(response);
      forceUpdate();
      setisLoading(false);
    } catch (error) {
      setisLoading(false);
      toast.error(error.response.data.message);
      console.log(error);
    }
  };
  const onSubmit2 = async (data) => {
    console.log(data);
    // const formData = new FormData();
    // formData.append("Name", data.name);
    // formData.append("City", data.city);
    // formData.append("Street", data.street);
    // formData.append("Logo", data.logo[0]);
    // formData.append("PhoneNumber", data.phone);
    // for (var pair of formData.entries()) {
    //   console.log(pair[0] + ", " + pair[1]);
    // }
    try {
      console.log(data);
      setisLoading(true);
      const JWTToken = Cookies.get("token");
      console.log(JWTToken);
      const response = await axios.post(
        "https://tazkartk-api.runasp.net/api/Companies",
        data,
        { headers: { Authorization: `Bearer ${JWTToken}` } }
      );
      addedSuccessfully();
      closeModal2();
      console.log(response);
      forceUpdate();
      setisLoading(false);
    } catch (error) {
      toast.error(error.response.data.message);
      setisLoading(false);
      console.log(error);
    }
  };
  const deleteCompany = async () => {
    try {
      setisDeleting(true);
      const response = await axios.delete(
        `https://tazkartk-api.runasp.net/api/Companies/${company.id}`
      );
      closeModal();
      setisDeleting(false);
      console.log(response);
      toast.success(response.data.message);
      forceUpdate();
    } catch (error) {
      setisDeleting(false);
      toast.error(error.response.data.message);
      console.log(error);
    }
  };
  return (
    <div className="flex flex-col  items-end w-full  xl:w-1/2 gap-4 ">
      <div className="flex items-center  justify-between w-full ">
        <button
          onClick={openModal2}
          className="bg-cyan-dark text-white p-4 rounded shadow-lg hover:cursor-pointer"
        >
          اضافة شركة
        </button>
        <p className="text-3xl text-cyan-dark font-bold ">الشركات</p>
      </div>
      {companies.map((company) => (
        <div
          key={company.id}
          className="flex  bg-white p-2 rounded-lg shadow-lg w-full justify-between items-center"
        >
          <div>
            <button
              onClick={() => openModal(company)}
              className="bg-cyan-dark text-white p-2 px-6 m-2 rounded hover:cursor-pointer"
            >
              تعديل
            </button>
            <button
              onClick={() => navigate(`/admin/${company.id}`)}
              className="bg-cyan-dark text-white p-2 px-6 rounded hover:cursor-pointer"
            >
              عرض الرحلات
            </button>
          </div>

          <div className="flex flex-col-reverse md:flex-row gap-2 items-center p-2">
            <h3 className="text-lg font-bold mt-2 text-center">
              {" "}
              {company.name}
            </h3>
            <img
              src={company.logo}
              alt="Go Bus"
              className="rounded-lg  size-16 xl:h-28"
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
          <h2 className="text-3xl font-bold text-center">اضافة شركة</h2>

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
              الاسم
              <input
                {...register2("name", {
                  required: "الاسم مطلوب",
                  minLength: {
                    value: 3,
                    message: "يجب أن يكون الاسم 3 أحرف على الأقل",
                  },
                })}
                className="w-full border p-2 my-2 rounded "
              />
              {errors2.name && (
                <p className="text-red-500 text-sm">{errors2.name.message}</p>
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
                className="w-full border p-2 my-2 rounded"
              />
              {errors2.email && (
                <p className="text-red-500 text-sm">{errors2.email.message}</p>
              )}
            </label>

            <label className="text-end w-full">
              رقم الهاتف
              <input
                {...register2("phone", {
                  required: "رقم الهاتف مطلوب",
                  pattern: {
                    value: /^[0-9]{11}$/,
                    message: "يجب أن يكون رقم الهاتف 11 رقماً ",
                  },
                })}
                className="w-full border p-2 my-2 rounded "
              />
              {errors2.phone && (
                <p className="text-red-500 text-sm">{errors2.phone.message}</p>
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
              {errors2.password && (
                <p className="text-red-500 text-sm">
                  {errors2.password.message}
                </p>
              )}
            </label>
            <div className="md:col-span-2">
              <label className=" w-full block text-end mb-1">
                تأكيد كلمة السر
              </label>

              <div className="relative">
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
              {errors2.confirmPassword && (
                <p className="text-red-500 text-end text-sm">
                  {errors2.confirmPassword.message}
                </p>
              )}
            </div>

            <div>
              <h3 className="text-xl text-end mb-2 text-cyan-dark ">العنوان</h3>
              <div className="flex gap-1.5">
                <label className="w-full text-end">
                  المدينة
                  <select
                    {...register2("city", {
                      required: "يجب اختيار المدينة",
                    })}
                    className="w-full border p-2 my-2 rounded text-end"
                  >
                    <option value="" disabled>
                      اختر المدينة
                    </option>

                    <option>القاهرة</option>
                    <option>الجيزة</option>
                    <option>الأسكندرية</option>
                    <option>الدقهلية</option>
                    <option>البحر الأحمر</option>
                    <option>البحيرة</option>
                    <option>الفيوم</option>
                    <option>الغربية</option>
                    <option>الإسماعلية</option>
                    <option>المنوفية</option>
                    <option>المنيا</option>
                    <option>القليوبية</option>
                    <option>الوادي الجديد</option>
                    <option>السويس</option>
                    <option>اسوان</option>
                    <option>اسيوط</option>
                    <option>بني سويف</option>
                    <option>بورسعيد</option>
                    <option>دمياط</option>
                    <option>الشرقية</option>
                    <option>جنوب سيناء</option>
                    <option>كفر الشيخ</option>
                    <option>مطروح</option>
                    <option>الأقصر</option>
                    <option>قنا</option>
                    <option>شمال سيناء</option>
                    <option>سوهاج</option>
                  </select>
                  {errors2.city && (
                    <p className="text-red-500 text-sm">
                      {errors2.city.message}
                    </p>
                  )}
                </label>
                <label className="text-end w-full">
                  الشارع
                  <input
                    {...register2("street", {
                      required: "يجب إدخال اسم الشارع",
                      minLength: {
                        value: 3,
                        message: "يجب أن يكون اسم الشارع 3 أحرف على الأقل",
                      },
                    })}
                    className="w-full border p-2 my-2 rounded"
                  />
                  {errors2.street && (
                    <p className="text-red-500 text-sm">
                      {errors2.street.message}
                    </p>
                  )}
                </label>
              </div>
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
        <div className="bg-white opacity-100 p-6 rounded-lg shadow-lg w-full  text-black">
          <h2 className="text-3xl font-bold text-center">تعديل البيانات</h2>

          <img
            src={companylogo || company.logo}
            alt=""
            className="rounded place-self-center size-36"
          />
          <form
            onSubmit={handleSubmit(onSubmit)}
            className="flex flex-col  gap-2 "
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
                className="w-full border p-2 my-2 rounded "
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
                className="w-full border p-2 my-2 rounded "
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
            <hr className="border m-2 w-3/4 place-self-end" />
            <div>
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
                    <option value="" disabled>
                      اختر المدينة
                    </option>

                    <option>القاهرة</option>
                    <option>الجيزة</option>
                    <option>الأسكندرية</option>
                    <option>الدقهلية</option>
                    <option>البحر الأحمر</option>
                    <option>البحيرة</option>
                    <option>الفيوم</option>
                    <option>الغربية</option>
                    <option>الإسماعلية</option>
                    <option>المنوفية</option>
                    <option>المنيا</option>
                    <option>القليوبية</option>
                    <option>الوادي الجديد</option>
                    <option>السويس</option>
                    <option>اسوان</option>
                    <option>اسيوط</option>
                    <option>بني سويف</option>
                    <option>بورسعيد</option>
                    <option>دمياط</option>
                    <option>الشرقية</option>
                    <option>جنوب سيناء</option>
                    <option>كفر الشيخ</option>
                    <option>مطروح</option>
                    <option>الأقصر</option>
                    <option>قنا</option>
                    <option>شمال سيناء</option>
                    <option>سوهاج</option>
                  </select>
                  {errors.city && (
                    <p className="text-red-500 text-sm">
                      {errors.city.message}
                    </p>
                  )}
                </label>
                <label className=" w-full text-end">
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
                    <p className="text-red-500 text-sm">
                      {errors.street.message}
                    </p>
                  )}
                </label>
              </div>
            </div>

            <input
              {...register("logo")}
              className="bg-cyan-dark rounded text-white p-4"
              type="file"
              name="logo"
              onChange={(e) => {
                const file = e.target.files?.[0];
                setLogo(file ? URL.createObjectURL(file) : undefined);
              }}
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
                disabled={isDeleting}
                onClick={deleteCompany}
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
