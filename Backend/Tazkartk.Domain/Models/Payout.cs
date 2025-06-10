using Tazkartk.Domain.Models.Enums;

namespace Tazkartk.Domain.Models
{
    public class Payout
    {
    public int Id { get; set; }
     public string PayoutId {  get; set; }
     public Double Amount {  get; set; }
     public DateTime Date {  get; set; }
     public PaymentMethods PaymentMethod { get; set; }
     public string? Wallet_Issuer {  get; set; }
     public string? WalletNumber {  get; set; }
     public string Status { get; set; }
     public Company Company {  get; set; }
     public int CompanyId {  get; set; }
   
    }
}
