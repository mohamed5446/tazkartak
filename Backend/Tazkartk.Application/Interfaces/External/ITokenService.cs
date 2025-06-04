using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tazkartk.Domain.Models;

namespace Tazkartk.Application.Interfaces.External
{
    public interface ITokenService
    {
        Task<string> GenerateToken(Account account);


    }
}
