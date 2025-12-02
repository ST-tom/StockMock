using Microsoft.AspNetCore.Http;
using MiniExcelLibs;
using MiniExcelLibs.Attributes;
using MiniExcelLibs.OpenXml;
using System.Reflection;

namespace StockMock.Infrastructure.Utils
{
    public partial class ExcelUtil
    {
        public async static Task<IEnumerable<T>> ReadExcelAsync <T>(IFormFile file, ExcelType excelType = ExcelType.XLSX, bool hasHeader = true, CancellationToken cancellationToken = default)
            where T : class, new()
        {
            // 1. 验证文件
            if (file == null || file.Length == 0)
                throw new ArgumentException("请上传有效的Excel文件");

            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            if (fileExtension != ".xlsx" && fileExtension != ".xls")
                throw new ArgumentException("仅支持.xlsx和.xls格式的Excel文件");

            using var stream = file.OpenReadStream();
            return await stream.QueryAsync<T>(excelType: excelType, hasHeader: hasHeader, cancellationToken: cancellationToken);
        }

        public static OpenXmlConfiguration InitConfiguration<T>(IEnumerable<T> data, Dictionary<string,string>? headers = null)
            where T : class, new()
        {
            OpenXmlConfiguration configuration = new OpenXmlConfiguration()
            {
                WriteEmptyStringAsNull = true
            };

            if (data.Count() > 10000000)
                throw new ArgumentException("数据量超过100w，请分批写入");

            var colmunList = new List<DynamicExcelColumn>();
            if (headers != null && headers.Any())
            {
                var type = typeof(T);
                Array.ForEach(type.GetProperties(BindingFlags.Public | BindingFlags.Instance), p => {
                    if (headers.TryGetValue(p.Name, out var header))
                        colmunList.Add(new DynamicExcelColumn(p.Name)
                        {
                            Name = header,
                            Ignore = false,
                        });
                    else
                        colmunList.Add(new DynamicExcelColumn(p.Name)
                        {
                            Ignore = true,
                        });
                });
                Array.ForEach(type.GetFields(BindingFlags.Public | BindingFlags.Instance), p => {
                    if (headers.TryGetValue(p.Name, out var header))
                        colmunList.Add(new DynamicExcelColumn(p.Name)
                        {
                            Name = header,
                            Ignore = false,
                        });
                    else
                        colmunList.Add(new DynamicExcelColumn(p.Name)
                        {
                            Ignore = true,
                        });
                });
                configuration.DynamicColumns = colmunList.ToArray();
            }

            return configuration;
        }
    }
}
