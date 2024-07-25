using HBS.TransformableViews;
using HBS.TransformableViews_Experience;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HBS.Xperience.TransformableViewsShared.Services
{
    internal class EncryptionService : IEncryptionService
    {
        public EncryptionService(string key)
        {
            _key = key;
        }
        private readonly string _key;
        public string EncryptString(string plainText)
        {
            if (!string.IsNullOrWhiteSpace(plainText))
            {
                byte[] iv = new byte[16];
                byte[] array;

                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(_key);
                    aes.IV = iv;

                    ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                            {
                                streamWriter.Write(plainText);
                            }

                            array = memoryStream.ToArray();
                        }
                    }
                }

                return Convert.ToBase64String(array);
            }
            else
            {
                return plainText;
            }
        }

        public string DecryptString(string cipherText)
        {
            if (!string.IsNullOrWhiteSpace(cipherText))
            {
                byte[] iv = new byte[16];
                try
                {
                    byte[] buffer = Convert.FromBase64String(cipherText);

                    using Aes aes = Aes.Create();
                    aes.Key = Encoding.UTF8.GetBytes(_key);
                    aes.IV = iv;
                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                    using MemoryStream memoryStream = new MemoryStream(buffer);
                    using CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
                    using StreamReader streamReader = new StreamReader(cryptoStream);
                    return streamReader.ReadToEnd();
                }
                catch (Exception)
                {
                    return cipherText;
                }
            }
            else
            {
                return cipherText;
            }
        }

        public TransformableViewInfo EncryptView(TransformableViewInfo view)
        {
            view.TransformableViewContent = EncryptString(view.TransformableViewContent);
            return view;
        }

        public TransformableViewInfo DecryptView(TransformableViewInfo view)
        {
            view.TransformableViewContent = DecryptString(view.TransformableViewContent);
            return view;
        }
    }
}
