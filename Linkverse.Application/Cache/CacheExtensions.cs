using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Linkverse.Application.Cache
{
    public static class CacheExtensions
    {
        public static string ToHashKey<T>(this T filterDto) where T : class
        {
            string json = JsonSerializer.Serialize(filterDto, new JsonSerializerOptions
            {
                WriteIndented = false
            });

            byte[] bytes = MD5.HashData(Encoding.UTF8.GetBytes(json));
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }
    }
}
