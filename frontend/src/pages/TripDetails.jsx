import axios from "axios";
import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router";
import { useAuthStore } from "../store/authStore";
import { motion } from "framer-motion";
import { Loader } from "lucide-react";

export default function TripDetails() {
  const { id: tripId } = useParams();
  const [trip, setTrip] = useState({});
  const [selectedSeats, setSelectedSeats] = useState([]);
  const { id } = useAuthStore();
  const navigate = useNavigate();
  const [isLoading, setisLoading] = useState(false);
  const [seatsData, setSeatData] = useState([
    { id: 1, booked: false },
    { id: 2, booked: false },
    { id: 3, booked: false },
    { id: 4, booked: false },
    { id: 5, booked: false },
    { id: 6, booked: false },
    { id: 7, booked: false },
    { id: 8, booked: false },
    { id: 9, booked: false },
    { id: 10, booked: false },
    { id: 11, booked: false },
    { id: 12, booked: false },
    { id: 13, booked: false },
    { id: 14, booked: false },
    { id: 15, booked: false },
    { id: 16, booked: false },
    { id: 17, booked: false },
    { id: 18, booked: false },
    { id: 19, booked: false },
  ]);
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
  const tripDetail = async () => {
    try {
      const res = await axios.get(
        `https://tazkartk-api.runasp.net/api/Trips/${tripId}`
      );
      console.log(tripId);
      setTrip(res.data);
      console.log(res);
      console.log(res.data);
      const updatedSeatsData = seatsData.map((seat) => ({
        ...seat,
        booked: res.data.bookedSeats.includes(seat.id),
      }));
      setSeatData(updatedSeatsData);
      console.log(updatedSeatsData);
    } catch (error) {
      console.log(error);
    }
  };
  const bookTicket = async () => {
    try {
      setisLoading(true);
      const data = {
        userId: id,
        tripId: parseInt(tripId),
        seatsNumbers: selectedSeats,
      };
      if (id === null) {
        navigate("/login");
      }

      console.log(data);
      const res = await axios.post(
        "https://tazkartk-api.runasp.net/api/Tickets",
        data
      );
      window.location.href = res.data.data;
      console.log(res);
      setisLoading(false);
    } catch (error) {
      setisLoading(false);
      console.log(error);
    }
  };
  useEffect(() => {
    tripDetail();
    console.log(trip);
  }, []);
  return (
    <div className="flex flex-col h-full m-4 gap-2 pb-6">
      <p className="text-end m-4 text-2xl  font-semibold">بيانات الرحلة</p>
      <div className=" flex w-full justify-around flex-row-reverse">
        <div>
          <p className="text-lg font-semibold text-center">الشركة</p>
          <p className="text-center">{trip.companyName}</p>
        </div>
        <div>
          <p className="text-lg font-semibold text-center">السعر</p>
          <p className="text-center">{trip.price} جنية </p>
        </div>
        <div>
          <p className="text-lg font-semibold text-center">من</p>
          <p className="text-center">{trip.from} </p>
        </div>
        <div>
          <p className="text-lg font-semibold text-center">الى</p>
          <p className="text-cente text-lg ">{trip.to} </p>
        </div>
        <div>
          <p className="text-lg font-semibold text-center">ميعاد المغادرة</p>
          <p className="text-center">{trip.departureDay}</p>
          <p className="text-center">{trip.departureDate}</p>
        </div>
        <div>
          <p className="text-lg font-semibold text-center">توقيت الوصول</p>
          <p className="text-center">{trip.arrivalDay}</p>
          <p className="text-center">{trip.arrivalTime}</p>
        </div>
        <div>
          <p className="text-lg font-semibold text-center">مكان التحرك</p>
          <p className="text-center">{trip.location}</p>
        </div>
      </div>

      <div className="flex flex-col justify-around items-center md:flex-row gap-2 h-full">
        {/* Seat Legend */}
        <div className="bg-white p-4 rounded shadow-md w-xs h-fit ">
          <h3 className="text-center font-semibold mb-2">Legend</h3>
          <div className="flex items-center gap-2 mb-2">
            <div className="w-6 h-6 bg-cyan-dark text-white flex justify-center items-center rounded">
              {`${trip.bookedSeats?.length}`}
            </div>
            <span>محجوز</span>
          </div>
          <div className="flex items-center gap-2">
            <div className="w-6 h-6 bg-gray-300 text-black flex justify-center items-center rounded">
              {`${19 - trip.bookedSeats?.length}`}
            </div>
            <span>متاح</span>
          </div>
        </div>
        {/* Seat Selection UI */}
        <div className="bg-white p-6 rounded shadow-md ">
          <h2 className="text-center font-semibold mb-4">اختر رقم مقعدك</h2>
          <div className="grid grid-cols-5 justify-center items-end">
            <div className="">
              {seatsData.slice(0, 5).map((seat) => (
                <button
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
              {seatsData.slice(5, 10).map((seat) => (
                <button
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
              {seatsData.slice(10, 11).map((seat) => (
                <button
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
              {seatsData.slice(11, 15).map((seat) => (
                <button
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
              {seatsData.slice(15, 19).map((seat) => (
                <button
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
          <motion.button
            whileHover={{ scale: 1.05 }}
            whileTap={{ scale: 0.95 }}
            className="mt-4 w-full bg-cyan-dark text-white py-2 rounded hover:bg-cyan-800 disabled:bg-gray-400"
            disabled={!selectedSeats}
            onClick={bookTicket}
          >
            {isLoading ? (
              <Loader className="animate-spin mx-auto" size={24} />
            ) : (
              " إتمام الحجز"
            )}
          </motion.button>
        </div>
      </div>
    </div>
  );
}
