import { useEffect, useState } from "react";
import { useAuthStore } from "../../store/authStore";
import axios from "axios";
import { Bounce, toast, ToastContainer } from "react-toastify";

export default function Tickets() {
  const [tickets, setTickets] = useState([]);
  const { id } = useAuthStore();
  const getTickets = async () => {
    try {
      const res = await axios.get(
        `https://tazkartk-api.runasp.net/api/${id}/tickets`
      );
      console.log(res);
      setTickets(res.data);
    } catch (error) {
      console.log(error);
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
      const res = await axios.post(
        `https://tazkartk-api.runasp.net/api/Tickets/${bookingId}/cancel`,
        {}
      );
      console.log(res);
    } catch (error) {
      console.log(error);
      errorCancelling(error.response.data.message);
    }
  };
  return (
    <div className="flex flex-col w-full md:w-7/10 gap-4">
      {tickets.map((ticket) => (
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
            <p className="text-lg font-semibold text-center">ميعاد المغادرة</p>
            <p className="text-center">{ticket.departureDay}</p>
            <p className="text-center">{ticket.departureDate}</p>
          </div>

          <button
            type="button"
            onClick={() => cancelTicket(ticket.bookingId)}
            className="bg-cyan-dark p-2 text-white rounded "
          >
            الغاء الحجز
          </button>
        </div>
      ))}
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
