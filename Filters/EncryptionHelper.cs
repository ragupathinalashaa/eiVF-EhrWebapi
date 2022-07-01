using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace eIVF.Filters
{

    public static class EncryptionHelper
    {
        #region Different Approaches
        //private static byte[] GenerateRandomData()
        //{
        //    byte[] data = new byte[256 * 1024 + 13];
        //    using (var gen = RandomNumberGenerator.Create())
        //        gen.GetBytes(data);

        //    return data;
        //}
        //public static string EncryptString(string text, string keyString)
        //{
        //    try
        //    {
        //        var key = Encoding.UTF8.GetBytes(keyString);
        //        using (var aesAlg = Aes.Create())
        //        {
        //            aesAlg.Mode = CipherMode.ECB;
        //            aesAlg.IV = new byte[16];
        //            using (var encryptor = aesAlg.CreateEncryptor(key, aesAlg.IV))
        //            {

        //                using (var msEncrypt = new MemoryStream())
        //                {
        //                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
        //                    using (var swEncrypt = new StreamWriter(csEncrypt))
        //                    {
        //                        swEncrypt.Write(text);
        //                    }

        //                    var iv = aesAlg.IV;

        //                    var decryptedContent = msEncrypt.ToArray();

        //                    var result = new byte[iv.Length + decryptedContent.Length];

        //                    Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
        //                    Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

        //                    return Convert.ToBase64String(result);
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.Message;
        //    }
        //}

        //public static string DecryptString(string cipherText, string keyString)
        //{
        //    var fullCipher = Convert.FromBase64String(cipherText);

        //    var iv = new byte[16];
        //    var cipher = new byte[16];

        //    Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
        //    Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, iv.Length);
        //    var key = Encoding.UTF8.GetBytes(keyString);

        //    using (var aesAlg = Aes.Create())
        //    {
        //        aesAlg.Mode = CipherMode.ECB;
        //        aesAlg.Padding = PaddingMode.Zeros;
        //        using (var decryptor = aesAlg.CreateDecryptor(key, iv))
        //        {
        //            string result;
        //            using (var msDecrypt = new MemoryStream(cipher))
        //            {
        //                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
        //                {
        //                    using (var srDecrypt = new StreamReader(csDecrypt))
        //                    {
        //                        result = srDecrypt.ReadToEnd();
        //                    }
        //                }
        //            }

        //            return result;
        //        }
        //    }
        //}


        //public static string Encrypt(string clearText, string key)
        //{
        //    string EncryptionKey = key; 
        //    byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
        //    using (Aes encryptor = Aes.Create())
        //    {
        //        Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
        //        encryptor.Key = pdb.GetBytes(32);
        //        encryptor.IV = pdb.GetBytes(16);
        //        encryptor.Mode = CipherMode.ECB;
        //        using (MemoryStream ms = new MemoryStream())
        //        {
        //            using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
        //            {
        //                cs.Write(clearBytes, 0, clearBytes.Length);
        //                cs.Close();
        //            }
        //            clearText = Convert.ToBase64String(ms.ToArray());
        //        }
        //    }
        //    return clearText;
        //}

        //public static string DecryptNew(string strKey, string strCipherText)
        //{
        //    byte[] arrbIv = new byte[16];
        //    //byte[] Key = ASCIIEncoding.UTF8.GetBytes(strKey);
        //    byte[] arrbBuffer = Convert.FromBase64String(strCipherText);

        //    using (Aes aes = Aes.Create())
        //    {
        //        aes.Key = ASCIIEncoding.UTF8.GetBytes(strKey);  // Convert.FromBase64String(strKey);
        //        aes.IV = arrbIv;
        //        aes.Padding = PaddingMode.Zeros;
        //        ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

        //        using (MemoryStream memoryStream = new MemoryStream(arrbBuffer))
        //        {
        //            using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream,
        //            decryptor, CryptoStreamMode.Read))
        //            {
        //                using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
        //                {
        //                    return streamReader.ReadToEnd();

        //                }
        //            }
        //        }
        //    }
        //}

        //public static string Decrypt(string cipherText, string key)
        //{
        //    string EncryptionKey = key;
        //    cipherText = cipherText.Replace(" ", "+");
        //    byte[] cipherBytes = Convert.FromBase64String(cipherText);
        //    //using (Aes encryptor = Aes.Create())
        //    //{
        //    //    //Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
        //    //    //encryptor.Key = pdb.GetBytes(32);
        //    //    //encryptor.IV = pdb.GetBytes(16);
        //    //    //encryptor.Padding = PaddingMode.None;

        //    //    Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, new byte[] {});
        //    //    encryptor.Padding = PaddingMode.None;
        //    //    using (MemoryStream ms = new MemoryStream())
        //    //    {
        //    //        using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
        //    //        {
        //    //            cs.Write(cipherBytes, 0, cipherBytes.Length);
        //    //            cs.Close();
        //    //        }
        //    //        cipherText = Encoding.Unicode.GetString(ms.ToArray());
        //    //    }
        //    //}
        //    string plainText = "";
        //    using (Aes encryptor = Aes.Create())
        //    {
        //        encryptor.Padding = PaddingMode.Zeros;
        //        //encryptor.Mode = CipherMode.CBC;
        //        /// encryptor.GenerateIV();
        //        /// 
        //       // encryptor.Key = pdb.GetBytes(32);
        //        ICryptoTransform Dec = encryptor.CreateDecryptor();
        //        using (MemoryStream ms = new MemoryStream(cipherBytes))
        //        {
        //            using (var cs = new CryptoStream(ms, Dec, CryptoStreamMode.Read))
        //            {
        //                byte[] decryptBlock = new byte[4096];
        //                MemoryStream decryptStream = new MemoryStream();
        //                int readBytes;
        //                while ((readBytes = cs.Read(decryptBlock, 0, 4096)) > 0)
        //                {
        //                    decryptStream.Write(decryptBlock, 0, readBytes);
        //                }
        //                cipherText = Encoding.Unicode.GetString(decryptStream.ToArray());
        //            }
        //        }
        //        encryptor.Clear();
        //    }

        //    return cipherText;
        //}
        #endregion

        private static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            // Check arguments.  
            if (cipherText == null || cipherText.Length <= 0)
            {
                throw new ArgumentNullException("cipherText");
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }

            // Declare the string used to hold  
            // the decrypted text.  
            string plaintext = null;

            // Create an RijndaelManaged object  
            // with the specified key and IV.  
            using (var rijAlg = new RijndaelManaged())
            {
                //Settings  
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;

                rijAlg.Key = key;
                rijAlg.IV = iv;

                // Create a decrytor to perform the stream transform.  
                var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                try
                {
                    // Create the streams used for decryption.  
                    using (var msDecrypt = new MemoryStream(cipherText))
                    {
                        using(var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {

                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                // Read the decrypted bytes from the decrypting stream  
                                // and place them in a string.  
                                plaintext = srDecrypt.ReadToEnd();

                            }

                        }
                    }
                }
                catch
                {
                    plaintext = "keyError";
                }
            }

            return plaintext;
        }
        private static string EncryptStringToBytes(string plainText, byte[] key, byte[] iv)
        {
            // Check arguments.  
            if (plainText == null || plainText.Length <= 0)
            {
                throw new ArgumentNullException("plainText");
            }
            if (key == null || key.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            if (iv == null || iv.Length <= 0)
            {
                throw new ArgumentNullException("key");
            }
            byte[] encrypted;
            // Create a RijndaelManaged object  
            // with the specified key and IV.  
            using (var rijAlg = new RijndaelManaged())
            {
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;

                rijAlg.Key = key;
                rijAlg.IV = iv;

                // Create a decrytor to perform the stream transform.  
                var encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption.  
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.  
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }

                }
          
                
            }

            // Return the encrypted bytes from the memory stream.  
            return Convert.ToBase64String(encrypted);
        }
        public static string Decrypt(string cipherText,string strKey)
        {
            var iv = Encoding.UTF8.GetBytes("8080808080808080");
            byte[] Key = ASCIIEncoding.UTF8.GetBytes(strKey);

            var encrypted = Convert.FromBase64String(cipherText);
            var decriptedFromJavascript = DecryptStringFromBytes(encrypted, Key, iv);

            return decriptedFromJavascript;
        }
        public static string Encrypt(string cipherText, string strKey)
        {
            var iv = Encoding.UTF8.GetBytes("8080808080808080");
            byte[] Key = ASCIIEncoding.UTF8.GetBytes(strKey);

            //var encrypted = Convert.FromBase64String(cipherText);
            var encryptedString = EncryptStringToBytes(cipherText, Key, iv);
          
            return string.Format(encryptedString);
        }

    }
}
