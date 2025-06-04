using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tazkartk.Application.Interfaces.External
{
    public interface IExcelService
    {

        public Task<IReadOnlyList<T>> Import<T>(IFormFile file, Func<IXLWorksheet, int, Task<T>> maprow) where T : class;
    }
}
