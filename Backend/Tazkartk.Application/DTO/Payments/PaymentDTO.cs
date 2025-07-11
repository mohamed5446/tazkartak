﻿namespace Tazkartk.Application.DTO.Payments
{
    public class PaymentDTO
    {
        public string PaymentIntentId { get; set; }
        public int PaymentId { get; set; }
        public bool IsRefunded { get; set; }

        public int? UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }

        public string time { get; set; }
        public string Method { get; set; }
        public double Amount { get; set; }
        public string CompanyName { get; set; }
        public List<int> SeatNumbers { get; set; }

    }
}
