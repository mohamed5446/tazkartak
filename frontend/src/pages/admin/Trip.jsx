import axios from "axios";
import { useEffect, useReducer, useState } from "react";
import { useParams } from "react-router";
import Modal from "react-modal";
import { TbSteeringWheel } from "react-icons/tb";
import { useForm } from "react-hook-form";
import { Loader } from "lucide-react";
import { motion } from "framer-motion";
const customStyles = {
  content: {
    top: "50%",
    left: "50%",
    right: "auto",
    bottom: "auto",
    padding: "0px",
    width: "90%",
    maxWidth: "45rem",
    transform: "translate(-50%, -50%)",
    borderRadius: "25px",
  },
  overlay: {
    backgroundColor: "rgba(189, 189, 189, 0.1)",
  },
};
function Trip() {
  const { tripId } = useParams();
  const [trip, setTrip] = useState({});
  const [people, setPeople] = useState([]);
  const [modalIsOpen, setIsOpen] = useState(false);
  const [selectedSeats, setSelectedSeats] = useState([]);
  const [ignored, forceUpdate] = useReducer((x) => x + 1, 0);
  const [isLoading, setisLoading] = useState(false);

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm();

  const [seatsData, setSeatData] = useState(
    Array.from({ length: 48 }, (_, i) => ({
      id: i + 1,
      booked: false,
    }))
  );
  function closeModal() {
    setIsOpen(false);
  }
  const handleSeatClick = (seat) => {
    if (!seat.booked) {
      setSelectedSeats((prevSelectedSeats) => {
        if (prevSelectedSeats.includes(seat.id)) {
          // If seat is already selected, remove it
          return prevSelectedSeats.filter((id) => id !== seat.id);
        } else {
          // Otherwise, add it to the selection
          return [...prevSelectedSeats, seat.id];
        }
      });
    }
  };
  const onSubmit = async (data) => {
    const completeData = {
      ...data,
      seats: selectedSeats,
    };
    try {
      setisLoading(true);

      await axios.post(
        `https://tazkartk-api.runasp.net/api/Tickets/${tripId}/guest`,
        completeData
      );
      reset(); // clear form
      setSelectedSeats([]); // clear seats
      closeModal();
      forceUpdate();
      setisLoading(false); // close modal
    } catch (error) {
      console.log(error);
      setisLoading(false);
    }
  };
  const getPeople = async () => {
    try {
      const res = await axios.get(
        `https://tazkartk-api.runasp.net/api/Trips/${tripId}/Passengers`
      );
      setPeople(res.data);
    } catch (error) {
      console.log(error);
    }
  };
  const tripDetail = async () => {
    try {
      const res = await axios.get(
        `https://tazkartk-api.runasp.net/api/Trips/${tripId}`
      );
      setTrip(res.data);

      const updatedSeatsData = seatsData.map((seat) => ({
        ...seat,
        booked: res.data.bookedSeats.includes(seat.id),
      }));
      setSeatData(updatedSeatsData);
    } catch (error) {
      console.log(error);
    }
  };
  const handleDelete = async (personId) => {
    const confirmed = window.confirm("هل أنت متأكد من حذف الحجز؟");
    if (!confirmed) return;

    try {
      await axios.delete(
        `https://tazkartk-api.runasp.net/api/Tickets/${personId}`
      );

      // Remove the person from state
      forceUpdate();
      // Optional: toast or alert
      alert("تم حذف الحجز بنجاح");
    } catch (error) {
      console.error("Error deleting person:", error);
      alert("حدث خطأ أثناء الحذف");
    }
  };

  useEffect(() => {
    tripDetail();
    getPeople();
    console.log(trip);
  }, [ignored]);
  return (
    <div className=" flex flex-col items-end">
      <p className="text-end m-4 text-2xl  font-semibold">بيانات الرحلة</p>

      <div className=" flex w-full flex-col   xl:flex-row-reverse gap-4 mb-2">
        <div className="flex flex-row w-full gap-2">
          <div className="w-full">
            <p className="text-lg font-semibold text-center">الشركة</p>
            <p className="text-center">{trip.companyName}</p>
          </div>
          <div className="w-full">
            <p className="text-lg font-semibold text-center">السعر</p>
            <p className="text-center">{trip.price} جنية</p>
          </div>
          <div className="w-full">
            <p className="text-lg font-semibold text-center">الى</p>
            <p className="text-center text-lg ">{trip.to} </p>
          </div>
          <div className="w-full">
            <p className="text-lg font-semibold text-center">من</p>
            <p className="text-center">{trip.from} </p>
          </div>
        </div>
        <div className="flex flex-row w-full gap-2">
          <div className="w-full">
            <p className="text-lg font-semibold text-center">ميعاد المغادرة</p>
            <p className="text-center">{trip.departureDay}</p>
            <p className="text-center">{trip.departureDate}</p>
          </div>
          {/* <div className="w-full">
      <p className="text-lg font-semibold text-center">توقيت الوصول</p>
      <p className="text-center">{trip.arrivalDay}</p>
      <p className="text-center">{trip.arrivalTime}</p>
    </div> */}
          <div className="w-full">
            <p className="text-lg font-semibold text-center">مكان التحرك</p>
            <p className="text-center">{trip.location}</p>
          </div>
        </div>
      </div>
      <div className="relative flex items-center justify-between w-full p-2 ">
        <button
          onClick={() => setIsOpen(true)}
          className="absolute left-1/2 transform -translate-x-1/2 p-2 bg-cyan-dark text-white rounded"
        >
          اضافة فرد
        </button>
        <p className="ml-auto text-xl font-semibold">الافراد</p>
      </div>
      {people.map((person, index) => (
        <div
          key={index}
          className="flex items-center justify-between flex-row-reverse w-3/4 max-w-3xl shadow p-2 rounded m-2"
        >
          <p>
            {person.firstName} {person.lastName}
          </p>
          <div>
            <p>ارقام المقاعد</p>
            {person.seats.map((seat, index) => (
              <span className="m-2" key={index}>
                {seat}
              </span>
            ))}
          </div>
          <div>
            <p>رقم الهاتف</p>
            <p>{person.phoneNumber}</p>
          </div>
          <button
            onClick={() => handleDelete(person.ticketId)}
            className="p-2 text-white rounded bg-cyan-dark"
          >
            حذف
          </button>
        </div>
      ))}
      <Modal
        isOpen={modalIsOpen}
        onRequestClose={closeModal}
        style={customStyles}
      >
        <div className="  p-6 rounded-lg shadow-lg w-full  text-black   space-y-4">
          {" "}
          <p className="text-2xl font-semibold text-center">اضافة فرد</p>
          <div className="">
            <form onSubmit={handleSubmit(onSubmit)} className="flex flex-col">
              <div className="flex w-full justify-between">
                <div className="grid grid-cols-5 justify-center items-end">
                  <div className="">
                    <TbSteeringWheel className="size-10 m-1" />

                    {seatsData.slice(0, 11).map((seat) => (
                      <button
                        type="button"
                        key={seat.id}
                        className={`w-10 m-1 h-10 flex items-center justify-center rounded ${
                          seat.booked
                            ? "bg-cyan-dark text-white cursor-not-allowed"
                            : selectedSeats.includes(seat.id)
                            ? "bg-blue-500 text-white"
                            : "bg-gray-300 text-black hover:bg-gray-400"
                        }`}
                        onClick={() => handleSeatClick(seat)}
                        disabled={seat.booked}
                      >
                        {seat.id}
                      </button>
                    ))}
                  </div>
                  <div className="">
                    {seatsData.slice(11, 22).map((seat) => (
                      <button
                        type="button"
                        key={seat.id}
                        className={`w-10 m-1 h-10 flex items-center justify-center rounded ${
                          seat.booked
                            ? "bg-cyan-dark text-white cursor-not-allowed"
                            : selectedSeats.includes(seat.id)
                            ? "bg-blue-500 text-white"
                            : "bg-gray-300 text-black hover:bg-gray-400"
                        }`}
                        onClick={() => handleSeatClick(seat)}
                        disabled={seat.booked}
                      >
                        {seat.id}
                      </button>
                    ))}
                  </div>
                  <div className="">
                    {seatsData.slice(22, 23).map((seat) => (
                      <button
                        type="button"
                        key={seat.id}
                        className={`w-10 m-1 h-10 flex items-center justify-center rounded ${
                          seat.booked
                            ? "bg-cyan-dark text-white cursor-not-allowed"
                            : selectedSeats.includes(seat.id)
                            ? "bg-blue-500 text-white"
                            : "bg-gray-300 text-black hover:bg-gray-400"
                        }`}
                        onClick={() => handleSeatClick(seat)}
                        disabled={seat.booked}
                      >
                        {seat.id}
                      </button>
                    ))}
                  </div>
                  <div className="">
                    {seatsData.slice(24, 36).map((seat) => (
                      <button
                        type="button"
                        key={seat.id}
                        className={`w-10 m-1 h-10 flex items-center justify-center rounded ${
                          seat.booked
                            ? "bg-cyan-dark text-white cursor-not-allowed"
                            : selectedSeats.includes(seat.id)
                            ? "bg-blue-500 text-white"
                            : "bg-gray-300 text-black hover:bg-gray-400"
                        }`}
                        onClick={() => handleSeatClick(seat)}
                        disabled={seat.booked}
                      >
                        {seat.id}
                      </button>
                    ))}
                  </div>
                  <div className="">
                    {seatsData.slice(36).map((seat) => (
                      <button
                        type="button"
                        key={seat.id}
                        className={`w-10 m-1 h-10 flex items-center justify-center rounded ${
                          seat.booked
                            ? "bg-cyan-dark text-white cursor-not-allowed"
                            : selectedSeats.includes(seat.id)
                            ? "bg-blue-500 text-white"
                            : "bg-gray-300 text-black hover:bg-gray-400"
                        }`}
                        onClick={() => handleSeatClick(seat)}
                        disabled={seat.booked}
                      >
                        {seat.id}
                      </button>
                    ))}
                  </div>
                </div>
                <div className="flex flex-col w-1/2 gap-2">
                  <label className="text-end">الاسم الاول</label>{" "}
                  <input
                    {...register("firstName", { required: true })}
                    className="shadow p-2 rounded border border-gray-300"
                    type="text"
                  />
                  {errors.firstName && (
                    <span className="text-red-500 text-sm text-end">مطلوب</span>
                  )}
                  <label className="text-end">اسم العائلة</label>{" "}
                  <input
                    {...register("lastName", { required: true })}
                    className="shadow p-2 rounded border border-gray-300"
                    type="text"
                  />
                  {errors.lastName && (
                    <span className="text-red-500 text-sm text-end">مطلوب</span>
                  )}
                  <label className="text-end">رقم الهاتف</label>{" "}
                  <input
                    {...register("phoneNumber", {
                      required: true,
                      pattern: /^[0-9]{10,15}$/,
                    })}
                    className="shadow p-2 rounded border border-gray-300"
                    type="text"
                  />
                  {errors.phoneNumber && (
                    <span className="text-red-500 text-sm text-end">
                      رقم هاتف غير صالح
                    </span>
                  )}
                </div>
              </div>

              <motion.button
                type="submit"
                disabled={isLoading}
                className="w-1/3 m-auto bg-cyan-dark text-white py-2 rounded hover:bg-cyan-900 mt-2"
              >
                {isLoading ? (
                  <Loader className="animate-spin mx-auto" size={24} />
                ) : (
                  "اضافة"
                )}
              </motion.button>
            </form>
          </div>
        </div>
      </Modal>
    </div>
  );
}

export default Trip;
