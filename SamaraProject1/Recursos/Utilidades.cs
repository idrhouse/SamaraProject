using System.Security.Cryptography;
using System.Text;


namespace SamaraProject1.Recursos
{
    public class Utilidades
    {
        public static string Cifrar(string clave)
        {
          StringBuilder sb = new StringBuilder();
            using (SHA256 sha256 = SHA256Managed.Create()) { 
            Encoding enc = Encoding.UTF8;
            byte[] result = sha256.ComputeHash(enc.GetBytes(clave));
            foreach (byte b in result)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
            
            }
        }


    }
}
