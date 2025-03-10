using Tazkartk.DTO.TripDTOs;
using Tazkartk.Models;

namespace Tazkartk.Mappers
{
    public static class TripMappers
    {
        public static TripDtos ToTripDto(this Trip TripModel )
        {
            return new TripDtos
            {
                TripId = TripModel.TripId,
                From = TripModel.From,
                To = TripModel.To,
                ArriveTime = TripModel.ArriveTime.ToString("dddd yyyy-MM-dd hh:mm tt"),
                Class = TripModel.Class,
                Date = TripModel.Date.ToString("dddd yyyy-MM-dd"),
                Time = TripModel.Time.ToString("hh:mm tt"),
                Avaliblility = TripModel.Avaliblility,
                Location = TripModel.Location,
                Price = TripModel.Price,
                CompanyName = TripModel.company.Name,
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
