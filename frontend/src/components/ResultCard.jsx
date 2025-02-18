export default function ResultsList({ trips }) {
  return (
    <div className="flex flex-col gap-6 w-full  ">
      {trips.map((trip, index) => (
        <div
          key={index}
          className="bg-white p-4 rounded shadow flex justify-between items-center flex-row-reverse"
        >
          <div className=" flex w-full justify-around flex-row-reverse">
            <div>
              <p className="text-lg font-semibold text-center">الشركة</p>
              <p className="text-center">{trip.company}</p>
            </div>
            <div>
              <p className="text-lg font-semibold text-center">السعر</p>
              <p className="text-center">{trip.price} جنية </p>
            </div>
            <div>
              <p className="text-lg font-semibold text-center">
                ميعاد المغادرة
              </p>
              <p className="text-center">{trip.departure}</p>
            </div>
            <div>
              <p className="text-lg font-semibold text-center">توقيت الوصول</p>
              <p className="text-center">{trip.arrival}</p>
            </div>
            <div>
              <p className="text-lg font-semibold text-center">مكان التحرك</p>
              <p className="text-center">{trip.location}</p>
            </div>
          </div>
          <button className="bg-cyan-dark text-white px-8 p-1 rounded">
            اختر
          </button>
        </div>
      ))}
    </div>
  );
}
