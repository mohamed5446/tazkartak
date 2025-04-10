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
            className="bg-white p-2 rounded shadow flex justify-between items-center w-full md:flex-row-reverse flex-col gap-6 md:gap-2"
          >
            <div className=" flex w-full flex-col   xl:flex-row-reverse gap-4">
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
                  <p className="text-lg font-semibold text-center">من</p>
                  <p className="text-center">{trip.from} </p>
                </div>
                <div className="w-full">
                  <p className="text-lg font-semibold text-center">الى</p>
                  <p className="text-center text-lg ">{trip.to} </p>
                </div>
              </div>
              <div className="flex flex-row w-full gap-2">
                <div className="w-full">
                  <p className="text-lg font-semibold text-center">
                    ميعاد المغادرة
                  </p>
                  <p className="text-center">{trip.departureDay}</p>
                  <p className="text-center">{trip.departureDate}</p>
                </div>
                <div className="w-full">
                  <p className="text-lg font-semibold text-center">
                    توقيت الوصول
                  </p>
                  <p className="text-center">{trip.arrivalDay}</p>
                  <p className="text-center">{trip.arrivalTime}</p>
                </div>
                <div className="w-full">
                  <p className="text-lg font-semibold text-center">
                    مكان التحرك
                  </p>
                  <p className="text-center">{trip.location}</p>
                </div>
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
