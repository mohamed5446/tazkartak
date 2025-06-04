using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tazkartk.Application.DTO
{
    public class dispurseresponse
    {
        public string TransactionId {  get; set; }
        public bool Success {  get; set; }  
        public string Status { get; set; }  
        public double amount {  get; set; }
        public string message { get; set; } 
    }
}
