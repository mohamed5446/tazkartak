import { motion } from "framer-motion";
import { useForm } from "react-hook-form";
import playStore from "../assets/play-store.62f26490 1.png";
import appStore from "../assets/app-store.81a46c9f 1.png";
import payMethodes from "../assets/Frame 21.png";
import HeroImage from "../assets/HeroImage.png";
import CompanyCard from "../components/CompanyCard";
import { useEffect, useState } from "react";
import axios from "axios";
import { createSearchParams, useNavigate } from "react-router";
const Home = () => {
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm();
  const [companies, setCompanies] = useState([]);
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
  useEffect(() => {
    try {
      fetchCompanies();
    } catch (error) {
      console.log(error);
    }
  }, []);
  const onSubmit = (data) => {
    navigate({
      pathname: "search-Result",
      search: createSearchParams({
        to: data.to,
        from: data.from,
        date: data.date || null,
      }).toString(),
    });
    console.log(data);
  };

  return (
    <motion.div
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.5 }}
      className=" mx-auto p-4 flex flex-col items-center 2xl:px-14"
    >
      {/* Hero Section */}
      <div className="md:relative md:inline flex justify-center w-full">
        <img
          className="z-10 hidden md:inline w-full h-full"
          src={HeroImage}
          alt=""
        />
        <div className="md:absolute  bottom-4 left-4 m-2 w-lg  text-center">
          <form
            onSubmit={handleSubmit(onSubmit)}
            className="bg-white p-6 rounded-lg shadow-lg mt-4 max-w-lg mx-auto"
          >
            <div className="flex flex-col md:flex-row gap-4">
              <div className="w-full md:w-1/2">
                <div>
                  <label className="block text-gray-700 mb-2">من</label>
                  <select
                    {...register("from", {
                      required: "حدد وجهة السفر",
                    })}
                    className="w-full border border-gray-300 p-2 rounded-lg text-end"
                    defaultValue=""
                  >
                    <option value="" disabled>
                      اختر وجهة السفر
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
                </div>
                {errors.to && (
                  <p className="text-red-500">{errors.to.message}</p>
                )}
              </div>
              <div className="w-full md:w-1/2">
                <div>
                  <label className="block text-gray-700 mb-2">إلى</label>
                  <select
                    {...register("to", {
                      required: "حدد وجهة السفر",
                    })}
                    className="w-full border border-gray-300 p-2 rounded-lg text-end"
                    defaultValue=""
                  >
                    <option value="" disabled>
                      اختر وجهة السفر
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
                </div>
                {errors.from && (
                  <p className="text-red-500">{errors.from.message}</p>
                )}
              </div>
            </div>
            <div className="">
              <div className="mt-4">
                <label className="block text-gray-700 mb-2">تاريخ السفر</label>
                <input
                  type="date"
                  {...register("date")}
                  className="w-full border border-gray-300 p-2 rounded-lg "
                />
              </div>
            </div>
            <button
              type="submit"
              className="mt-6 w-full bg-cyan-dark text-white py-2 rounded-lg hover:bg-cyan-900"
            >
              عرض الرحلات المتوفرة
            </button>
          </form>
        </div>
      </div>

      {/* Payment Section */}
      <div className="text-center shadow-gray-400 shadow-lg rounded-lg m-8 p-6">
        <h2 className="text-xl font-bold">وسائل دفع مريحة و آمنة</h2>
        <div className="mt-2 flex justify-center space-x-4">
          <img src={payMethodes} alt="Visa" className="h-10" />
        </div>
      </div>

      {/* Mobile App Section */}
      <div className="text-center m-8 shadow-gray-400 shadow-lg rounded-lg p-6 max-w-lg bg-white">
        <h2 className="text-xl font-bold">ابحث عن رحلتك في متجر التطبيقات</h2>
        <div className="flex justify-center space-x-4 mt-4">
          <img src={appStore} alt="App Store" className="h-12" />
          <img src={playStore} alt="Google Play" className="h-12" />
        </div>
      </div>

      {/* Bus Companies Section */}
      <div className="mt-8">
        <h2 className="text-xl font-bold text-center">الشركات</h2>
        <div className="grid grid-cols-1 md:grid-cols-2 2xl:grid-cols-3 gap-4 mt-4">
          <CompanyCard Children={companies} />
        </div>
      </div>
    </motion.div>
  );
};

export default Home;
