

export default function TripCartCo({ trip, onEdit })  {
    if(!trip)  {
        return <p>Loading...</p>;
      }
  return (
    <div className="p-4 bg-white rounded-lg flex gap-24 items-center ml-12 mb-4" 
    style={{width:"90%",boxShadow:"rgba(0, 0, 0, 0.2) 0px 5px 17px"}}>
      <button className="bg-blue-500 text-white px-3 py-1 rounded" onClick={() => onEdit(trip)}>
        تعديل
      </button>
        <p><strong>من:</strong> {trip.from}</p>
        <p><strong>إلى:</strong> {trip.to}</p>
        <p><strong>الموعد:</strong> {trip.departureTime}</p>
        <p><strong>الوصول:</strong> {trip.arrivalTime}</p>
        <p><strong>السعر:</strong> {trip.price} جم</p>
      
      
    </div>
  )
}



