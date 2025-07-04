import { useEffect, useState } from "react";
import axios from "axios";
import { Bounce, toast, ToastContainer } from "react-toastify";
import { Loader } from "lucide-react";
import { motion } from "framer-motion";
import { useParams } from "react-router";
export default function Tickets() {
  const [tickets, setTickets] = useState([]);
  const { id } = useParams();
  const [loadingStates, setisLoading] = useState({});
  const [isfetching, setIsFetching] = useState(false);

  const getTickets = async () => {
    setIsFetching(true);
    try {
      const res = await axios.get(
        `https://tazkartk-api.runasp.net/api/${id}/tickets`
      );
      console.log(res);
      setTickets(res.data);
    } catch (error) {
      console.log(error);
    } finally {
      setIsFetching(false);
    }
  };
  const errorCancelling = (message) =>
    toast.error(message, {
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
  useEffect(() => {
    getTickets();
  }, []);
  const cancelTicket = async (bookingId) => {
    try {
      setisLoading((prev) => ({ ...prev, [bookingId]: true }));
      const res = await axios.post(
        `https://tazkartk-api.runasp.net/api/Tickets/${bookingId}/cancel`,
        {}
      );
      setisLoading((prev) => ({ ...prev, [bookingId]: false }));
      console.log(res);
    } catch (error) {
      console.log(error);
      setisLoading((prev) => ({ ...prev, [bookingId]: false }));
      errorCancelling(error.response.data.message);
    }
  };
  if (isfetching) return <p className="text-center p-4">جاري التحميل...</p>;

  return (
    <div className="flex flex-col w-full   p-4 gap-4">
      {tickets.length >= 1 ? (
        tickets.map((ticket) => (
          <div
            key={ticket.bookingId}
            className=" flex   justify-around flex-row-reverse items-center shadow p-2 rounded"
          >
            <div>
              <p className="text-lg font-semibold text-center">الشركة</p>
              <p className="text-center">{ticket.companyName}</p>
            </div>
            <div>
              <p className="text-lg font-semibold text-center"> عدد التذاكر</p>
              <p className="text-center">{ticket.seatsNumbers?.length}</p>
            </div>

            <div>
              <p className="text-lg font-semibold text-center">من</p>
              <p className="text-center">{ticket.from} </p>
            </div>
            <div>
              <p className="text-lg font-semibold text-center">الى</p>
              <p className="text-cente text-lg ">{ticket.to} </p>
            </div>
            <div>
              <p className="text-lg font-semibold text-center">
                ميعاد المغادرة
              </p>
              <p className="text-center">{ticket.departureDay}</p>
              <p className="text-center">{ticket.departureDate}</p>
            </div>

            <button type="button"> </button>
            <motion.button
              type="button"
              onClick={() => cancelTicket(ticket.bookingId)}
              disabled={loadingStates[ticket.bookingId]}
              className="bg-cyan-dark p-2 text-white rounded "
            >
              {loadingStates[ticket.bookingId] ? (
                <Loader className="animate-spin  mx-auto" size={24} />
              ) : (
                "الغاء الحجز"
              )}
            </motion.button>
          </div>
        ))
      ) : (
        <p className="text-center text-2xl "> لا يوجد تذاكر</p>
      )}
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
