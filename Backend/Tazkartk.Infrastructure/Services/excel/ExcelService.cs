using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tazkartk.Application.Interfaces;

namespace Tazkartk.Infrastructure.excel
{
    internal class ExcelService : IExcelService
    {
    
        public async Task<IReadOnlyList<T>> Import<T>(IFormFile file, Func<IXLWorksheet, int, Task<T>> mapRow) where T : class
        {
            var results = new List<T>();

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            using var workbook = new XLWorkbook(stream);

            foreach (var worksheet in workbook.Worksheets)
            {
                for (int row = 2; row <= worksheet.LastRowUsed().RowNumber(); row++)
                {
                    if (worksheet.Cell(row, 1).IsEmpty()) continue;
                    try
                    {
                        var result = await mapRow(worksheet, row);
                        results.Add(result);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"{row}: {ex.Message}");
                    }
                }
            }

            return results;
        }
    }
}
