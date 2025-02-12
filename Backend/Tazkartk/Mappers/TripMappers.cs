using Tazkartk.DTO;

using Tazkartk.Models;

namespace Tazkartk.Mappers
{
    public static class TripMappers
    {
        public static TripDtos ToTripDto(this Trip TripModel)
        {
            return new TripDtos
            {
                TripId = TripModel.TripId,
                From = TripModel.From,
                To = TripModel.To,
                ArriveTime = TripModel.ArriveTime,
                Class = TripModel.Class,
                Date = TripModel.Date,
                Time = TripModel.Time,
                Avaliblility = TripModel.Avaliblility,
                Location = TripModel.Location,
                Price = TripModel.Price
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
