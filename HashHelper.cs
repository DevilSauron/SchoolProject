using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SkolniProjekt
{
    static class HashHelper
    {
        public static string Hash(string password)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] encoded = Encoding.UTF8.GetBytes(password);
                byte[] ret = md5.ComputeHash(encoded);

                StringBuilder sb = new StringBuilder();

                foreach (byte t in ret)
                {
                    sb.Append(t.ToString("x2"));
                }

                return sb.ToString();
            }
        }
    }
}
