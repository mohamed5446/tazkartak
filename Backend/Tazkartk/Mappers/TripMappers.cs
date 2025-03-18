using System.Globalization;
using Tazkartk.DTO.TripDTOs;
using Tazkartk.Models;
using Tazkartk.Models.Enums;

namespace Tazkartk.Mappers
{
    public static class TripMappers
    {
        public static TripDtos ToTripDto(this Trip TripModel )
        {
            var arabicCulture = new CultureInfo("ar-SA");
            arabicCulture.DateTimeFormat.Calendar = new GregorianCalendar();
            return new TripDtos
            {
                TripId = TripModel.TripId,
                From = TripModel.From,
                To = TripModel.To,
                ArriveTime = TripModel.ArriveTime.ToString("dddd yyyy-MM-dd hh:mm tt", arabicCulture),
                Class = TripModel.Class,
                Date = TripModel.Date.ToString("dddd yyyy-MM-dd",arabicCulture),
                Time = TripModel.Time.ToString("hh:mm tt", arabicCulture),
                Avaliblility = TripModel.Avaliblility,
                Location = TripModel.Location,
                Price = TripModel.Price,
                CompanyName = TripModel.company.Name,
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
