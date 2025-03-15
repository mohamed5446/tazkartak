import axios from "axios";
import { useEffect, useReducer, useState } from "react";
import { useForm } from "react-hook-form";
import Modal from "react-modal";
import { motion } from "framer-motion";
import { Loader } from "lucide-react";
import { Bounce, ToastContainer, toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";

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
  const [isLoading, setisLoading] = useState(false);
  const [isDeleting, setisDeleting] = useState(false);

  const [ignored, forceUpdate] = useReducer((x) => x + 1, 0);
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
      console.log(response);
      setisDeleting(false);
      forceUpdate();
    } catch (error) {
      setisDeleting(false);
      console.log(error);
    }
  };
  return (
    <div className="flex flex-col m-4 items-end  gap-4 p-2">
      <div className="flex items-center  justify-between w-sm  md:w-2xl lg:w-1/2 ">
        <button className="bg-cyan-dark text-white p-4 rounded shadow-lg hover:cursor-pointer">
          اضافة مستخدم
        </button>
        <p className="text-3xl text-cyan-dark font-bold ">الشركات</p>
      </div>
      {users.map((user) => (
        <div
          key={user.id}
          className="flex bg-white p-4 rounded-lg shadow-lg  w-sm  md:w-2xl lg:w-1/2 justify-between items-center"
        >
          <button
            onClick={() => openModal(user)}
            className="bg-cyan-dark text-white p-4 px-6 rounded hover:cursor-pointer"
          >
            تعديل
          </button>
          <div className="flex gap-4 items-center">
            <h3 className="text-lg font-bold mt-2 text-center">
              {user.firstName} {user.lastName}
            </h3>
            <img
              src={user.photoUrl}
              alt="user image"
              className="rounded-lg h-28"
            />
          </div>
        </div>
      ))}

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
                className="w-full border p-2 my-2 rounded text-end"
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
                className="w-full border p-2 my-2 rounded text-end"
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
