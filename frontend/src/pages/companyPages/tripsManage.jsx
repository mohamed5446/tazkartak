import TripCartCo from "./tripCartCo";
import { useState } from "react";
export default function tripsManage() {
  const [trips, setTrips] = useState([
    { id: 1, from: "القاهرة", to: "الإسكندرية", departureTime: "7:00 مساءً", arrivalTime: "9:00 مساءً", price: 350 },
    { id: 2, from: "الإسكندرية", to: "أسوان", departureTime: "8:00 صباحًا", arrivalTime: "4:00 مساءً", price: 800 },
    { id: 3, from: "أسيوط", to: "القاهرة", departureTime: "5:30 مساءً", arrivalTime: "10:00 مساءً", price: 450 },
    { id: 4, from: "المنصورة", to: "شرم الشيخ", departureTime: "6:00 صباحًا", arrivalTime: "1:00 مساءً", price: 600 },
    { id: 5, from: "الفيوم", to: "الأقصر", departureTime: "9:00 مساءً", arrivalTime: "6:00 صباحًا", price: 700 },
    { id: 6, from: "طنطا", to: "الإسكندرية", departureTime: "3:00 مساءً", arrivalTime: "5:00 مساءً", price: 250 },
    { id: 7, from: "الإسكندرية", to: "القاهرة", departureTime: "10:00 صباحًا", arrivalTime: "12:00 مساءً", price: 350 },
    { id: 8, from: "القاهرة", to: "الغردقة", departureTime: "11:00 مساءً", arrivalTime: "5:00 صباحًا", price: 500 },
    { id: 9, from: "الزقازيق", to: "بورسعيد", departureTime: "1:00 ظهرًا", arrivalTime: "3:00 عصرًا", price: 200 },
    { id: 10, from: "السويس", to: "الإسماعيلية", departureTime: "2:00 مساءً", arrivalTime: "4:00 مساءً", price: 180 },
    { id: 11, from: "دمياط", to: "الإسكندرية", departureTime: "7:30 صباحًا", arrivalTime: "10:00 صباحًا", price: 300 },
    { id: 12, from: "الأقصر", to: "أسوان", departureTime: "4:00 مساءً", arrivalTime: "7:00 مساءً", price: 400 },
    { id: 13, from: "مرسى مطروح", to: "الإسكندرية", departureTime: "5:00 مساءً", arrivalTime: "9:00 مساءً", price: 500 },
    { id: 14, from: "الغردقة", to: "القاهرة", departureTime: "9:00 صباحًا", arrivalTime: "3:00 مساءً", price: 550 },
    { id: 15, from: "المنوفية", to: "القاهرة", departureTime: "8:00 مساءً", arrivalTime: "9:30 مساءً", price: 150 },
    { id: 16, from: "القاهرة", to: "مرسى علم", departureTime: "10:00 مساءً", arrivalTime: "6:00 صباحًا", price: 750 },
    { id: 17, from: "سوهاج", to: "الإسكندرية", departureTime: "3:00 مساءً", arrivalTime: "11:00 مساءً", price: 900 },
    { id: 18, from: "بني سويف", to: "القاهرة", departureTime: "6:00 صباحًا", arrivalTime: "8:00 صباحًا", price: 200 },
  ]);
  const handleEdit = (trip) => {
    console.log("تعديل الرحلة:", trip);
  };
  return (
    <div className=" "
     style={{width:"83%",margin:"auto",textAlign:"center",marginTop:"50px"}}>
      <div className="flex items-center justify-between px-4 py-4">
        <button className="bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded">
          اضافة رحلة
        </button>
        <h2 className="text-2xl font-bold mb-4 text-center">الرحلات</h2>
      </div>
      {trips.map((trip) => (
        <TripCartCo key={trip.id} trip={trip} onEdit={() => {handleEdit(trip)}}/>
      ))}
   
    </div>
  )
}
