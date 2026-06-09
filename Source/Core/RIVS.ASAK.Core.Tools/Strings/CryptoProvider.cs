using System;
using System.Security.Cryptography;
using System.Text;

namespace RIVS.ASAK.Core.Tools.Strings
{
    public static class CryptoProvider
    {
        /// <summary>
        /// Метод получения шестнадцатеричного хэша строки.
        /// </summary>
        /// <param name="data">Исходная строка.</param>
        /// <returns>Хэш строки.</returns>
        public static string ComputeHashHex(string data)
        {
            var arr = ComputeHashArrayPrivate(data);
            var res = new StringBuilder();
            foreach (var symb in arr)
            {
                res.Append(symb.ToString("x2"));
            }
            return res.ToString();
        }

        /// <summary>
        /// Метод получения хэша.
        /// </summary>
        /// <param name="data">Исходная строка.</param>
        /// <returns>Возвращает хэш в виде массива байт.</returns>
        private static byte[] ComputeHashArrayPrivate(string data)
        {
            var md5 = new MD5CryptoServiceProvider();
            if (!string.IsNullOrEmpty(data))
            {
                var bytes = Encoding.UTF8.GetBytes(data);
                return md5.ComputeHash(bytes);
            }
            if (data == string.Empty)
            {
                return Array.Empty<byte>();
            }

            throw new ArgumentException("Wrong data type");
        }
    }
}
