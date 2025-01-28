import { useForm } from "react-hook-form";
import companyImage from "../assets/3 1.png";
import playStore from "../assets/play-store.62f26490 1.png";
import appStore from "../assets/app-store.81a46c9f 1.png";
import payMethodes from "../assets/Frame 21.png";
import HeroImage from "../assets/HeroImage.png";
const Home = () => {
  const { register, handleSubmit } = useForm();

  const onSubmit = (data) => {
    console.log(data);
  };

  return (
    <div className=" mx-auto p-4 flex flex-col items-center 2xl:px-14">
      {/* Hero Section */}
      <div className="relative w-full">
        <img className="z-10 w-full h-full" src={HeroImage} alt="" />
        <div className="absolute   bottom-4 left-4 m-2 w-lg text-white text-center">
          <form
            onSubmit={handleSubmit(onSubmit)}
            className="bg-white p-6 rounded-lg shadow-lg mt-4 max-w-2xl mx-auto"
          >
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label className="block text-gray-700 mb-2">إلى</label>
                <select
                  {...register("to")}
                  className="w-full border border-gray-300 p-2 rounded-lg"
                >
                  <option value="الإسكندرية">الإسكندرية</option>
                  <option value="القاهرة">القاهرة</option>
                </select>
              </div>
              <div>
                <label className="block text-gray-700 mb-2">من</label>
                <select
                  {...register("from")}
                  className="w-full border border-gray-300 p-2 rounded-lg"
                >
                  <option value="القاهرة">القاهرة</option>
                  <option value="الإسكندرية">الإسكندرية</option>
                </select>
              </div>
            </div>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div className="mt-4">
                <label className="block text-gray-700 mb-2">تاريخ العودة</label>
                <input
                  type="date"
                  {...register("date")}
                  className="w-full border border-gray-300 p-2 rounded-lg"
                />
              </div>
              <div className="mt-4">
                <label className="block text-gray-700 mb-2">تاريخ السفر</label>
                <input
                  type="date"
                  {...register("date")}
                  className="w-full border border-gray-300 p-2 rounded-lg"
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
          <div className="bg-white p-4 rounded-lg shadow-lg">
            <img src={companyImage} alt="Go Bus" className="rounded-lg" />
            <h3 className="text-lg font-bold mt-2 text-center">أوتوبيسي</h3>
          </div>
          <div className="bg-white p-4 rounded-lg shadow-lg">
            <img src={companyImage} alt="Go Bus" className="rounded-lg" />
            <h3 className="text-lg font-bold mt-2 text-center">أوتوبيسي</h3>
          </div>
          <div className="bg-white p-4 rounded-lg shadow-lg">
            <img src={companyImage} alt="Otobeesy" className="rounded-lg" />
            <h3 className="text-lg font-bold mt-2 text-center">جو باص</h3>
          </div>
        </div>
      </div>
    </div>
  );
};

export default Home;
