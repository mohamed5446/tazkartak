import axios from "axios";
import { motion } from "framer-motion";
import { Loader } from "lucide-react";
import { useEffect, useReducer, useState } from "react";
import { useParams } from "react-router";
import { Bounce, toast, ToastContainer } from "react-toastify";
import Modal from "react-modal";
import "react-toastify/dist/ReactToastify.css";
import TripForm from "../../components/TripForm";
const customStyles = {
  content: {
    top: "50%",
    left: "50%",
    right: "auto",
    bottom: "auto",
    padding: "0px",
    width: "60%",
    maxWidth: "45rem",
    transform: "translate(-50%, -50%)",
    borderRadius: "25px",
  },
  overlay: {
    backgroundColor: "rgba(189, 189, 189, 0.1)",
  },
};
Modal.setAppElement("#root");

export default function CompanyTrips() {
  const [trips, setTrips] = useState({});
  const { id } = useParams();
  const [ignored, forceUpdate] = useReducer((x) => x + 1, 0);
  const [isDeleting, setisDeleting] = useState({});
  const [modalIsOpen, setIsOpen] = useState(false);
  function closeModal() {
    setIsOpen(false);
  }
  const getTrips = async () => {
    try {
      const res = await axios.get(
        `https://tazkartk-api.runasp.net/api/${id}/Trips`
      );
      console.log(res.data);
      setTrips(res.data);
      console.log(trips);
    } catch (error) {
      console.log(error);
    }
  };
  const cancleTrip = async (id) => {
    try {
      setisDeleting((prev) => ({ ...prev, [id]: true }));
      const res = await axios.delete(
        `https://tazkartk-api.runasp.net/api/Trips/${id}`
      );
      toast.success("تم حذف الرحلة بنجاح!", {
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
      forceUpdate();
      console.log(res);
      setisDeleting((prev) => ({ ...prev, [id]: false }));
    } catch (error) {
      setisDeleting((prev) => ({ ...prev, [id]: false }));
      toast.error(error.response.data.message, {
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
      console.log(error);
    }
  };
  const addTrip = async (tripData) => {
    console.log(tripData);
    try {
      const res = await axios.post(
        `https://tazkartk-api.runasp.net/api/Trips/${id}`,
        tripData
      );
      console.log(res);
      forceUpdate();
      setIsOpen(false);
    } catch (error) {
      console.log(error);
    }
  };
  useEffect(() => {
    getTrips();
  }, [ignored]);
  return (
    <div className="flex flex-col gap-6 w-full p-4  items-end ">
      <ToastContainer position="top-right" autoClose={3000} />
      <button
        className="bg-cyan-dark  text-white p-2 rounded"
        type="button"
        onClick={() => setIsOpen(true)}
      >
        اضافة رحلة
      </button>
      {trips.length >= 1 ? (
        trips.map((trip, index) => (
          <div
            key={index}
            className="bg-white p-4 rounded shadow flex justify-between items-center w-full flex-row-reverse"
          >
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
                <p className="text-lg font-semibold text-center">
                  ميعاد المغادرة
                </p>
                <p className="text-center">{trip.departureDay}</p>
                <p className="text-center">{trip.departureDate}</p>
              </div>
              {/* <div>
                <p className="text-lg font-semibold text-center">
                  توقيت الوصول
                </p>
                <p className="text-center">{trip.arrivalDay}</p>
                <p className="text-center">{trip.arrivalTime}</p>
              </div> */}
              <div>
                <p className="text-lg font-semibold text-center">مكان التحرك</p>
                <p className="text-center">{trip.location}</p>
              </div>
            </div>

            <motion.button
              type="button"
              disabled={isDeleting[trip.tripId]}
              onClick={() => cancleTrip(trip.tripId)}
              className="bg-red-500 text-white px-8 p-2 rounded"
            >
              {isDeleting[trip.tripId] ? (
                <Loader className="animate-spin mx-auto" size={24} />
              ) : (
                "حذف"
              )}
            </motion.button>
          </div>
        ))
      ) : (
        <p className="text-lg text-center">لا توجد رحلات</p>
      )}
      <Modal
        isOpen={modalIsOpen}
        onRequestClose={closeModal}
        style={customStyles}
      >
        <TripForm onSubmit={addTrip} />
      </Modal>
    </div>
  );
}
