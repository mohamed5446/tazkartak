import { useEffect, useState } from "react";
import axios from "axios";
import { toast } from "react-toastify";

function CompanyPayoutTable({ companyId, refreshTrigger }) {
  const [payouts, setPayouts] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchPayouts = async () => {
      try {
        const res = await axios.get(
          `https://tazkartk-api.runasp.net/api/Companies/${companyId}/Payouts`
        );
        setPayouts(res.data);
      } catch (error) {
        console.log(error);
        toast.error("فشل في تحميل سجل السحوبات");
      } finally {
        setLoading(false);
      }
    };

    fetchPayouts();
  }, [companyId, refreshTrigger]);

  if (loading) return <p className="text-center p-4">جاري التحميل...</p>;

  if (payouts.length === 0) {
    return <p className="text-center p-4">لا يوجد سجلات سحب حالياً</p>;
  }

  return (
    <div className="overflow-x-auto max-w-5xl mx-auto mt-8 shadow-md rounded-xl ">
      <table className="min-w-full divide-y divide-gray-200 bg-white text-right">
        <thead className="bg-cyan-dark text-white">
          <tr>
            <th className="px-4 py-3 text-sm font-medium">التاريخ</th>
            <th className="px-4 py-3 text-sm font-medium">المبلغ</th>
            <th className="px-4 py-3 text-sm font-medium">طريقة الدفع</th>
            <th className="px-4 py-3 text-sm font-medium">رقم المحفظة</th>
            <th className="px-4 py-3 text-sm font-medium">الحالة</th>
          </tr>
        </thead>
        <tbody className="divide-y divide-gray-100">
          {payouts.map((payout) => (
            <tr key={payout.payoutId} className="hover:bg-gray-50">
              <td className="px-4 py-3">{payout.date}</td>
              <td className="px-4 py-3 text-cyan-dark font-semibold">
                {payout.amount} ج.م
              </td>
              <td className="px-4 py-3">{payout.paymentMethod}</td>
              <td className="px-4 py-3">{payout.walletNumber}</td>
              <td className="px-4 py-3">
                <span
                  className={`px-2 py-1 rounded-full text-xs font-medium ${
                    payout.status === "Succeeded"
                      ? "bg-green-100 text-green-800"
                      : "bg-yellow-100 text-yellow-800"
                  }`}
                >
                  {payout.status === "Succeeded" ? "ناجح" : payout.status}
                </span>
              </td>
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  );
}

export default CompanyPayoutTable;
