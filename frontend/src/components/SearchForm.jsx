import { useForm } from "react-hook-form";
export default function SearchForm() {
  const { register, handleSubmit } = useForm();

  const onSubmit = (data) => {
    console.log(data);
  };
  return (
    <div className="bottom-4 left-4 m-2 w-lg text-white text-center h-fit shadow">
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
        <button
          type="submit"
          className="mt-6 w-full bg-cyan-dark text-white py-2 rounded-lg hover:bg-cyan-900"
        >
          عرض الرحلات المتوفرة
        </button>
      </form>
    </div>
  );
}
