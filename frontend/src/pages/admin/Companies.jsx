import { useEffect, useReducer, useState } from "react";
import axios from "axios";
import Modal from "react-modal";
import { useForm } from "react-hook-form";
import { motion } from "framer-motion";
import { Loader } from "lucide-react";
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
  const [company, setCompany] = useState({});
  const [modalIsOpen, setIsOpen] = useState(false);
  const [modal2IsOpen, set2IsOpen] = useState(false);
  const [ignored, forceUpdate] = useReducer((x) => x + 1, 0);

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
  useEffect(() => {
    try {
      fetchCompanies();
    } catch (error) {
      console.log(error);
    }
  }, [ignored]);

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
    setValue("name", "");
    setValue("email", "");
    setValue("phone", "");
    setValue("city", "");
    setValue("street", "");
    set2IsOpen(true);
  }

  function closeModal2() {
    setLogo(undefined);
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
      closeModal();
      console.log(response);
      forceUpdate();
      setisLoading(false);
    } catch (error) {
      setisLoading(false);
      console.log(error);
    }
  };
  const deleteCompany = async () => {
    try {
      const response = await axios.delete(
        `https://tazkartk-api.runasp.net/api/Companies/${company.id}`
      );
      closeModal();
      console.log(response);
      forceUpdate();
    } catch (error) {
      console.log(error);
    }
  };
  return (
    <div className="flex flex-col m-4 items-end  gap-4 p-2">
      <div className="flex items-center  justify-between w-sm  md:w-2xl lg:w-1/2 ">
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
          className="flex bg-white p-4 rounded-lg shadow-lg  w-sm  md:w-2xl lg:w-1/2 justify-between items-center"
        >
          <button
            onClick={() => openModal(company)}
            className="bg-cyan-dark text-white p-4 px-6 rounded hover:cursor-pointer"
          >
            تعديل
          </button>
          <div className="flex gap-4 items-center">
            <h3 className="text-lg font-bold mt-2 text-center">
              {" "}
              {company.name}
            </h3>
            <img src={company.logo} alt="Go Bus" className="rounded-lg h-28" />
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

          <img
            src={
              companylogo ||
              "https://cdn.pixabay.com/photo/2015/10/05/22/37/blank-profile-picture-973460_1280.png"
            }
            alt=""
            className="rounded place-self-center size-36"
          />
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
            </label>

            <input
              {...register("logo")}
              className="bg-cyan-dark rounded text-white p-4"
              type="file"
              accept="image/*"
              onChange={(e) => {
                const file = e.target.files?.[0];
                setLogo(file ? URL.createObjectURL(file) : undefined);
              }}
            />

            <div className="flex gap-2 ">
              <button
                type="submit"
                className="w-full bg-cyan-dark text-white py-2 rounded hover:bg-cyan-900"
              >
                اضافة
              </button>
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
            src={companylogo || company.logo}
            alt=""
            className="rounded place-self-center size-36"
          />
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
            </label>

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
              <button
                type="button"
                onClick={deleteCompany}
                className="w-full bg-red-500 text-white py-2 rounded hover:bg-red-800"
              >
                حذف
              </button>
            </div>
          </form>
        </div>
      </Modal>
    </div>
  );
}
