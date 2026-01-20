using System.Text;
using TS.Shared.Extension;

namespace TS.Shared.Util
{
    public enum FileType
    {
        Json = 1,
        Xml = 2,
        Txt = 3,
        Csv = 4
    }

    public class FileUtil
    {
        /// <summary>
        /// 映射枚举到对应的文件后缀（带点，统一小写）
        /// </summary>
        private static Dictionary<FileType, string?> FileTypeExtensionDic => new()
        {
            { FileType.Json, ".json" },
            { FileType.Xml, ".xml" },
            { FileType.Txt, ".txt" },
            { FileType.Csv, ".csv" }
        };

        public static string GetExtDateTimeGuidFileName(string fileName)
        {
            if (fileName.IsNullOrWhiteSpace())
                return string.Empty;

            return $"{Path.GetFileNameWithoutExtension(fileName)}_{DateTime.Now.ToDateTimeString()}_{Guid.NewGuid():N}{Path.GetExtension(fileName)}";
        }

        /// <summary>
        ///  保存文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="fileName"></param>
        /// <param name="content"></param>
        /// <param name="fileType"></param>
        /// <param name="encoding">默认utf8 无BOM</param>
        /// <param name="bufferSize"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task SaveFileAsync(string? filePath, string? fileName, string? content, FileType fileType = FileType.Json, Encoding? encoding = null, int bufferSize = 4096, CancellationToken cancellationToken = default)
        {
            if (content.IsNullOrEmpty())
                return;

            filePath = filePath?.Trim();
            filePath ??= Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "files");
            Directory.CreateDirectory(filePath!);

            if (fileName.IsNullOrWhiteSpace())
                fileName = $"{DateTime.Now:yyyyMMddHHmmssfff}_{Guid.NewGuid():N}.{FileTypeExtensionDic[fileType]}";
            else
                fileName = Path.GetFileNameWithoutExtension(fileName) + FileTypeExtensionDic[fileType];

            var fileFullPath = Path.Combine(filePath!, fileName!);

            using FileStream fileStream = new(fileFullPath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize, FileOptions.Asynchronous);
            using StreamWriter streamWriter = new(fileStream, encoding ?? new UTF8Encoding(false));
            await streamWriter.WriteAsync(content.AsMemory(), cancellationToken);
        }
    }
}
