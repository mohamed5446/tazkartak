using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Tazkartk.Domain.Models.Enums
{
    public enum PaymentStatus
    {
        [Display(Name = "معلق")]
        Pending = 0,

        [Display(Name = "اكتمل")]
        Succeeded = 1,

        [Display(Name = "فشل")]
        Failed = 2
    }
}
