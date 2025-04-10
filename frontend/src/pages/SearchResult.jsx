import ResultsList from "../components/ResultCard";
import { useEffect, useState } from "react";
import { useSearchParams } from "react-router";
import axios from "axios";
import { useForm } from "react-hook-form";
import { motion } from "framer-motion";
import { Loader } from "lucide-react";
export default function SearchResult() {
  const [searchParams, setSearchParams] = useSearchParams();
  const [trips, setTrips] = useState([]);
  const { register, handleSubmit, setValue } = useForm();
  const [isLoading, setisLoading] = useState(false);

  const to = searchParams.get("to");
  const from = searchParams.get("from");
  const date = searchParams.get("date");
  const getTrips = async (to, date, from) => {
    try {
      setisLoading(true);
      const res = await axios.get(
        "https://tazkartk-api.runasp.net/api/Trips/Search",
        { params: { to: to, from: from, date: date } }
      );
      console.log(res.data);
      setTrips(res.data);
      setisLoading(false);
      console.log(trips);
    } catch (error) {
      setisLoading(false);
      console.log(error);
    }
  };
  useEffect(() => {
    console.log("params changed");
    console.log(to, from, date);
    setValue("to", to);
    setValue("from", from);
    setValue("date", date);
    getTrips(to, date, from);
  }, [to, from, date]);
  const onSubmit = (data) => {
    console.log(data);
    setSearchParams({ to: data.to, from: data.from, date: data.date });
  };
  return (
    <div className="p-4 flex md:flex-row flex-col-reverse w-full gap-2">
      <ResultsList className="grow" trips={trips} />
      <div className="bottom-4 left-4   rounded-lg text-center h-fit shadow">
        <form
          onSubmit={handleSubmit(onSubmit)}
          className=" p-4  shadow-lg  mx-auto"
        >
          <div className="flex flex-col md:flex-row gap-4">
            <div className="w-full md:w-1/2">
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

            <div className="w-full md:w-1/2">
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
          </div>

          <div className="grid grid-cols-1  gap-4">
            <div className="mt-4">
              <label className="block text-gray-700 mb-2">تاريخ السفر</label>
              <input
                type="date"
                {...register("date")}
                className="w-full border border-gray-300 p-2 rounded-lg"
              />
            </div>
          </div>
          <motion.button
            whileHover={{ scale: 1.05 }}
            whileTap={{ scale: 0.95 }}
            disabled={isLoading}
            type="submit"
            className="mt-6 w-full bg-cyan-dark text-white py-2 rounded-lg hover:bg-cyan-900"
          >
            {isLoading ? (
              <Loader className="animate-spin mx-auto" size={24} />
            ) : (
              "عرض الرحلات المتوفرة"
            )}
          </motion.button>
        </form>
      </div>
    </div>
  );
}
