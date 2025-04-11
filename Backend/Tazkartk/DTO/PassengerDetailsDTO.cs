﻿namespace Tazkartk.DTO
{
    public class PassengerDetailsDTO
    {
        public int? Id { get; set; } 
        public string FirstName {  get; set; }
        public string LastName { get; set; }
        public string? Email { get; set; }
        public string PhoneNumber {  get; set; }
        public List<int> Seats { get; set; }
    }
}
