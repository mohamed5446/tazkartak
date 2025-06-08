import axios from "axios";
import { useEffect, useReducer, useState } from "react";
import { useAuthStore } from "../../store/authStore";
import { useForm } from "react-hook-form";
import { toast, ToastContainer } from "react-toastify";
import { motion } from "framer-motion";
import CompanyPayoutTable from "./CompanyPayoutTable";

function Earnings() {
  const [earnings, setEarnings] = useState(0);
  const { id } = useAuthStore();
  const [isLoading, setIsLoading] = useState(false);
  const [ignored, forceUpdate] = useReducer((x) => x + 1, 0);
  const [refresh, setRefresh] = useState(false);
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm();
  const onSubmit = async (data) => {
    setIsLoading(true);

    console.log("Withdraw request submitted:", data);
    // You can call your API here
    try {
      await axios.post(
        `https://tazkartk-api.runasp.net/api/Companies/${id}/withraw`,
        data
      );
      setRefresh(true);
      forceUpdate();
      toast.success("تم السحب بنجاح");
    } catch (error) {
      console.error("Withdrawal error:", error);
      toast.error(error.response.data.message);
    } finally {
      setIsLoading(false);
    }
  };
  const getEarnings = async () => {
    try {
      const res = await axios.get(
        `https://tazkartk-api.runasp.net/api/Companies/${id}`
      );
      setEarnings(res.data.balance);
    } catch (error) {
      console.log(error);
    }
  };
  useEffect(() => {
    getEarnings();
  }, [ignored]);
  return (
    <div className="max-w-2xl  bg-white rounded-3xl  p-6 space-y-6 text-center w-full">
      <p className="text-3xl font-bold text-gray-800">الرصيد</p>
      <p className="text-4xl font-bold text-cyan-dark">{earnings}</p>

      <p className="text-lg font-semibold text-gray-600">سحب الرصيد</p>

      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
        <select
          className="w-full border border-gray-300 rounded-xl p-2 text-right bg-gray-100 focus:outline-none focus:ring-2 focus:ring-cyan-500"
          {...register("issuer", { required: "برجاء اختيار طريقة السحب" })}
        >
          <option value="vodafone">vodafone</option>
          <option value="etisalat">etisalat</option>
          <option value="orange">orange</option>
          <option value="bank_wallet">bank_wallet</option>
        </select>
        {errors.method && (
          <p className="text-red-500 text-sm text-right">
            {errors.method.message}
          </p>
        )}
        <div className="text-right">
          <label className="block text-sm font-medium text-gray-700 mb-1">
            رقم الهاتف
          </label>
          <input
            type="text"
            className="w-full border border-gray-300 rounded-xl p-2 bg-gray-100 focus:outline-none focus:ring-2 focus:ring-cyan-500 text-right"
            {...register("walletPhoneNumber", {
              required: "رقم الهاتف مطلوب",
              pattern: {
                value: /^[0-9]{10,15}$/,
                message: "رقم غير صالح",
              },
            })}
          />
          {errors.phone && (
            <p className="text-red-500 text-sm text-right">
              {errors.phone.message}
            </p>
          )}
        </div>

        <button
          type="submit"
          className="w-full flex justify-center items-center bg-cyan-dark text-white py-2 px-4 rounded-xl font-semibold hover:bg-cyan-800 transition disabled:opacity-70"
          disabled={isLoading}
        >
          {isLoading ? (
            <motion.div
              className="w-5 h-5 border-2 border-white border-t-transparent rounded-full animate-spin"
              initial={{ rotate: 0 }}
              animate={{ rotate: 360 }}
              transition={{ repeat: Infinity, duration: 0.6, ease: "linear" }}
            />
          ) : (
            "تأكيد السحب"
          )}
        </button>
      </form>
      <CompanyPayoutTable companyId={id} refreshTrigger={refresh} />
      <ToastContainer />
    </div>
  );
}

export default Earnings;
