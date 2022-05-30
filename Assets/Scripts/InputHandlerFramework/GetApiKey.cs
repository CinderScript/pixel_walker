using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


public class GptApiKey
{
    public string ApiKey { get; private set; }
    public string FilePath { get; }

    public GptApiKey(string path)
    {
        FilePath = path;
    }

    // method to encypt the key by cryptostream and memorystream, then write it on a file
    public void SaveKeyToFile(string key)
    {
        try
        {
            string publickey = "12345678";
            string secretkey = "87654321";
            byte[] secretkeyByte = { };
            secretkeyByte = System.Text.Encoding.UTF8.GetBytes(secretkey);
            byte[] publickeybyte = { };
            publickeybyte = System.Text.Encoding.UTF8.GetBytes(publickey);
            MemoryStream ms = null;
            CryptoStream cs = null;
            byte[] inputbyteArray = System.Text.Encoding.UTF8.GetBytes(key);
            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                ms = new MemoryStream();
                cs = new CryptoStream(ms, des.CreateEncryptor(publickeybyte, secretkeyByte), CryptoStreamMode.Write);
                cs.Write(inputbyteArray, 0, inputbyteArray.Length);
                cs.FlushFinalBlock();
                key = Convert.ToBase64String(ms.ToArray());
                ApiKey = key;
            }
            using (StreamWriter wr = new StreamWriter(FilePath))
            {
                wr.WriteLine(ApiKey);
            }
        }
        catch (Exception)
        {
            throw;
        }
    }

    // method to retrieve the encrypted key from the file then decrypt it
    public string GetKeyFromFile()
    {
        try
        {
            string key;
            using (StreamReader rd = new StreamReader(FilePath))
            {
                key = rd.ReadLine();
            }
            string publickey = "12345678";
            string secretkey = "87654321";
            byte[] privatekeyByte = { };
            privatekeyByte = System.Text.Encoding.UTF8.GetBytes(secretkey);
            byte[] publickeybyte = { };
            publickeybyte = System.Text.Encoding.UTF8.GetBytes(publickey);
            MemoryStream ms = null;
            CryptoStream cs = null;
            byte[] inputbyteArray = new byte[key.Replace(" ", "+").Length];
            inputbyteArray = Convert.FromBase64String(key.Replace(" ", "+"));
            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                ms = new MemoryStream();
                cs = new CryptoStream(ms, des.CreateDecryptor(publickeybyte, privatekeyByte), CryptoStreamMode.Write);
                cs.Write(inputbyteArray, 0, inputbyteArray.Length);
                cs.FlushFinalBlock();
                Encoding encoding = Encoding.UTF8;
                key = encoding.GetString(ms.ToArray());
            }
            return key;
        }
        catch (Exception)
        {
            throw;
        }
    }

}

