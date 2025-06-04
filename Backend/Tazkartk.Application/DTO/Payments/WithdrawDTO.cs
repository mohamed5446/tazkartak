using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tazkartk.Application.DTO.Payments
{
    public class WithdrawDTO
    {
        [Required]
        public string issuer { get; set; }
        [Required, RegularExpression(@"^01[0125][0-9]{8}$", ErrorMessage = "يرجى إدخال رقم هاتف صحيح")]

        public string WalletPhoneNumber { get; set; }
    }
}
