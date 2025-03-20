import { useNavigate } from "react-router";

export default function ResultsList({ trips }) {
  const navigate = useNavigate();
  const showTrip = (id) => {
    navigate(`/trip-details/${id}`);
  };
  return (
    <div className="flex flex-col gap-6 w-full  ">
      {trips.length >= 1 ? (
        trips.map((trip, index) => (
          <div
            key={index}
            className="bg-white p-4 rounded shadow flex justify-between items-center flex-row-reverse"
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
              <div>
                <p className="text-lg font-semibold text-center">
                  توقيت الوصول
                </p>
                <p className="text-center">{trip.arrivalDay}</p>
                <p className="text-center">{trip.arrivalTime}</p>
              </div>
              <div>
                <p className="text-lg font-semibold text-center">مكان التحرك</p>
                <p className="text-center">{trip.location}</p>
              </div>
            </div>
            <button
              type="button"
              onClick={() => showTrip(trip.tripId)}
              className="bg-cyan-dark text-white px-8 p-2 rounded"
            >
              اختر
            </button>
          </div>
        ))
      ) : (
        <p className="text-lg text-center">لا توجد رحلات للمعايير المجددة</p>
      )}
    </div>
  );
}
