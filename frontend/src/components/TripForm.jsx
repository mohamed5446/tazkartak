import { motion } from "framer-motion";
import { Loader } from "lucide-react";
import { useState } from "react";
import { useForm } from "react-hook-form";
import { toast } from "react-toastify";
export default function TripForm({ onSubmit }) {
  const [isLoading, setisLoading] = useState(false);
  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
  } = useForm({
    defaultValues: {
      avaliblility: true, // Default to available
    },
  });

  const submitForm = async (data) => {
    try {
      setisLoading(true);
      await onSubmit(data); // Call parent function with form data
      toast.success("تم إضافة الرحلة بنجاح!");
      reset(); // Clear form after submission
    } catch (error) {
      console.log(error);
      toast.error("حدث خطا اثناء اضافة الرحله");
    } finally {
      setisLoading(false);
    }
  };

  return (
    <form
      onSubmit={handleSubmit(submitForm)}
      className="bg-white shadow-md rounded-lg p-6 space-y-6 w-full max-w-4xl mx-auto"
    >
      <div className="grid grid-cols-2 gap-6 text-end">
        {/* From */}
        <div>
          <label className="block text-gray-700 font-semibold">من</label>
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
          {errors.from && (
            <p className="text-red-500 text-sm">{errors.from.message}</p>
          )}
        </div>

        {/* To */}
        <div>
          <label className="block text-gray-700 font-semibold">إلى</label>
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
          {errors.to && (
            <p className="text-red-500 text-sm">{errors.to.message}</p>
          )}
        </div>

        {/* Availability */}
        <div className="hidden">
          <input
            type="checkbox"
            {...register("avaliblility")}
            className="mr-2"
          />
          <label className=" text-gray-700 font-semibold">متاح</label>
        </div>

        {/* Class */}
        <div>
          <label className="block text-gray-700 font-semibold">
            فئة الرحلة
          </label>
          <select
            {...register("class", { required: "يجب اختيار فئة الرحلة" })}
            className="w-full p-2 border rounded mt-1"
          >
            <option value="">اختر الفئة</option>
            <option value="Economy">اقتصادي</option>
            <option value="Business">رجال الأعمال</option>
            <option value="VIP">VIP</option>
          </select>
          {errors.class && (
            <p className="text-red-500 text-sm">{errors.class.message}</p>
          )}
        </div>

        {/* Date */}
        <div>
          <label className="block text-gray-700 font-semibold">
            تاريخ الرحلة
          </label>
          <input
            type="date"
            {...register("date", { required: "تاريخ الرحلة مطلوب" })}
            className="w-full p-2 border rounded mt-1"
          />
          {errors.date && (
            <p className="text-red-500 text-sm">{errors.date.message}</p>
          )}
        </div>

        {/* Time */}
        <div>
          <label className="block text-gray-700 font-semibold">
            وقت المغادرة
          </label>
          <input
            type="time"
            step="1"
            {...register("time", { required: "وقت المغادرة مطلوب" })}
            className="w-full p-2 border rounded mt-1"
          />
          {errors.time && (
            <p className="text-red-500 text-sm">{errors.time.message}</p>
          )}
        </div>

        {/* Arrival Time */}
        {/* <div>
          <label className="block text-gray-700 font-semibold">
            وقت الوصول
          </label>
          <input
            type="datetime-local"
            {...register("arriveTime", { required: "وقت الوصول مطلوب" })}
            className="w-full p-2 border rounded mt-1"
          />
          {errors.arriveTime && (
            <p className="text-red-500 text-sm">{errors.arriveTime.message}</p>
          )}
        </div> */}

        {/* Location */}
        <div>
          <label className="block text-gray-700 font-semibold">
            مكان التحرك
          </label>
          <input
            type="text"
            {...register("location", { required: "مكان التحرك مطلوب" })}
            className="w-full p-2 border rounded mt-1"
          />
          {errors.location && (
            <p className="text-red-500 text-sm">{errors.location.message}</p>
          )}
        </div>

        {/* Price */}
        <div className="col-span-2">
          <label className="block text-gray-700 font-semibold">السعر</label>
          <input
            type="number"
            {...register("price", {
              required: "السعر مطلوب",
              min: { value: 0, message: "يجب أن يكون السعر رقمًا موجبًا" },
            })}
            className="w-full p-2 border rounded mt-1"
          />
          {errors.price && (
            <p className="text-red-500 text-sm">{errors.price.message}</p>
          )}
        </div>
      </div>

      {/* Submit Button */}
      <motion.button type="submit"></motion.button>
      <motion.button
        disabled={isLoading}
        className="bg-cyan-dark text-white w-full p-2 rounded hover:bg-cyan-900 transition"
      >
        {isLoading ? (
          <Loader className="animate-spin mx-auto" size={24} />
        ) : (
          "إضافة الرحلة"
        )}
      </motion.button>
    </form>
  );
}
