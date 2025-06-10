using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tazkartk.Application.DTO
{
    public class MessageDTO
    {
        public int MessageId {  get; set; }
       public string Subject { get; set; }    
       public string Body { get; set; }   
       public string Name {  get; set; }
       public string Email {  get; set; }
    }
}
