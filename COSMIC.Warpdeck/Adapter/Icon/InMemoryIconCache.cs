using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using COSMIC.Warpdeck.Domain.Button;
using COSMIC.Warpdeck.Domain.Icon;

namespace COSMIC.Warpdeck.Adapter.Icon
{
    
    public class InMemoryIconCache : IIconCache
    {
        private Dictionary<string, KeyIcon> _cache = new();
        
        public bool DoesCacheHaveIcon(ButtonModel buttonModel)
        {
            string hash = CreateMD5(JsonSerializer.Serialize(buttonModel));
            return _cache.ContainsKey(hash);
        }

        public KeyIcon GetIcon(ButtonModel buttonModel)
        {
            string hash = CreateMD5(JsonSerializer.Serialize(buttonModel));
            return _cache[hash];
        }

        public KeyIcon SetIcon(ButtonModel model, KeyIcon icon)
        {
            string hash = CreateMD5(JsonSerializer.Serialize(model));
            _cache[hash] = icon;
            return icon;
        }

        public void Clear()
        {
            _cache.Clear();
        }


        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}