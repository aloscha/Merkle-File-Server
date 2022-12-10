using System.Collections.Generic;
using System.Text;

namespace MerkleFileServer.Helpers
{
    public class Encryptor
    {
        public static byte[] ToSha256(byte[] randomString)
        {
            var crypt = new System.Security.Cryptography.SHA256Managed();
            byte[] crypto = crypt.ComputeHash(randomString);
            return crypto;
        }
        public static string ByteArrayToString(byte[] randomString)
        { 
            var builder = new StringBuilder();
            for (int i = 0; i < randomString.Length; i++)
            {
                builder.Append(randomString[i].ToString("x2"));
            }
            return builder.ToString();
        }
        public static byte[] StringToByteArray(string randomString)
        {
            var hexindex = new Dictionary<string, byte>();
            for (var i = 0; i <= 255; i++)
            {
                hexindex.Add(i.ToString("X2").ToLower(), (byte)i);
            }

            var hexres = new List<byte>();
            for (var i = 0; i < randomString.Length; i += 2)
            {
                hexres.Add(hexindex[randomString.Substring(i, 2)]);
            }

            return hexres.ToArray();
        }
    }
}
