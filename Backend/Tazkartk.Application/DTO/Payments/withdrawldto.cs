using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tazkartk.Application.DTO.Payments
{
    public class withdrawldto
    {
        public string Issuer { get; set; }
        public string walletnumber { get; set; }
        public double amount { get; set; }

    }
}
