using System.ComponentModel.DataAnnotations;

namespace Tazkartk.Google
{
    public class GooglesigninDTO
    {
     [Required] 
     public string IdToken { get; set; }
    }
}
