using System.Globalization;
using Tazkartk.DTO.TripDTOs;
using Tazkartk.Models;
using Tazkartk.Models.Enums;

namespace Tazkartk.Mappers
{
    public static class TripMappers
    {
        private const char RightToLeftCharacter = (char)0x200F;

        public static TripDtos ToTripDto(this Trip TripModel )
        {
            var arabicCulture = new CultureInfo("ar-SA");
            arabicCulture.DateTimeFormat.Calendar = new GregorianCalendar();
            return new TripDtos
            {
                TripId = TripModel.TripId,
                From = TripModel.From,
                To = TripModel.To,
               
                Class = TripModel.Class,
                DepartureDate= TripModel.Date.ToString("yyyy-MM-dd", arabicCulture),
                DepartureTime= RightToLeftCharacter + TripModel.Time.ToString("hh:mm tt", arabicCulture),
                DepartureDay=TripModel.Date.ToString("dddd", arabicCulture),
                ArrivalDate= TripModel.ArriveTime.ToString("yyyy-MM-dd", arabicCulture),
                ArrivalTime= RightToLeftCharacter + TripModel.ArriveTime.ToString("hh:mm tt", arabicCulture),
                ArrivalDay=TripModel.ArriveTime.ToString("dddd", arabicCulture),

                //DepartureTime = new DTO.Date
                //{
                //    date = TripModel.Date.ToString("yyyy-MM-dd", arabicCulture),
                //    time=RightToLeftCharacter+ TripModel.Time.ToString("hh:mm tt", arabicCulture),
                //    day = TripModel.Date.ToString("dddd", arabicCulture)

                //},
                //ArriveTime = new DTO.Date
                //{
                //    date = TripModel.ArriveTime.ToString("yyyy-MM-dd", arabicCulture),
                //    time = RightToLeftCharacter + TripModel.ArriveTime.ToString("hh:mm tt", arabicCulture),
                //    day = TripModel.ArriveTime.ToString("dddd", arabicCulture)

                //},
                Avaliblility = TripModel.Avaliblility,
                Location = TripModel.Location,
                Price = TripModel.Price,
                CompanyName = TripModel.company.Name,
                //Date = TripModel.Date.ToString("dddd yyyy-MM-dd",arabicCulture),
                //Time = TripModel.Time.ToString("hh:mm tt", arabicCulture),
                // TripModel.ArriveTime.ToString("dddd yyyy-MM-dd hh:mm tt", arabicCulture),
                //Seats = TripModel.bookings.SelectMany(b => b.seats.Where(s => s.State == SeatState.Available).Select(s => s.Number)).ToList(),
            };
        }
        public static Trip ToTripFromCreateDtos(this CreateTripDtos TripDtos)
        {
            return new Trip
            {
                From = TripDtos.From,
                To = TripDtos.To,
                Avaliblility = TripDtos.Avaliblility,
                Class = TripDtos.Class,
                Date = TripDtos.Date,
                Time = TripDtos.Time,
                ArriveTime = TripDtos.ArriveTime,
                Location = TripDtos.Location,
                Price = TripDtos.Price

            };
        }
    }
}
