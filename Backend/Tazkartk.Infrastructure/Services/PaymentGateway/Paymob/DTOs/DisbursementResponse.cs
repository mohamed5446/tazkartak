using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tazkartk.Infrastructure.Services.PaymentGateway.Paymob.DTOs
{
    public class DisbursementResponse
    {
        [JsonProperty("transaction_id")]
        public string TransactionId { get; set; }

        [JsonProperty("issuer")]
        public string Issuer { get; set; }

        [JsonProperty("msisdn")]
        public string Msisdn { get; set; }

        [JsonProperty("amount")]
        public double Amount { get; set; }

        [JsonProperty("full_name")]
        public string FullName { get; set; }

        [JsonProperty("disbursement_status")]
        public string DisbursementStatus { get; set; }

        [JsonProperty("status_code")]
        public string StatusCode { get; set; }

        [JsonProperty("fees")]
        public double Fees { get; set; }

        [JsonProperty("vat")]
        public double Vat { get; set; }

        [JsonProperty("reference_id")]
        public string ReferenceId { get; set; }

        [JsonProperty("status_description")]
        public string StatusDescription { get; set; }

        [JsonProperty("aman_cashing_details")]
        public object AmanCashingDetails { get; set; }

        [JsonProperty("client_transaction_reference")]
        public object ClientTransactionReference { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [JsonProperty("accept_balance_transfer_id")]
        public object AcceptBalanceTransferId { get; set; }

        [JsonProperty("reference")]
        public object Reference { get; set; }

        [JsonProperty("comment1")]
        public string Comment1 { get; set; }
    }

}
