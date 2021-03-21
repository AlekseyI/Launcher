using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CommonClasses.CryptHash
{
    public class Crypt
    {
        public byte[] Key { get; set; }

        public byte[] Iv { get; set; }

        public Crypt(byte[] key, byte[] iv)
        {
            Key = key;
            Iv = iv;
        }

        public Crypt(byte[] key)
        {
            Key = key;
        }

        public Crypt() { }

        public byte[] Encode(byte[] data)
        {
            byte[] result;
            using (var stream = new MemoryStream())
            {
                using (var crypt = new RijndaelManaged())
                {
                    if (Key is null)
                    {
                        crypt.GenerateKey();
                        Key = crypt.Key;
                    }
                    if (Iv is null)
                    {
                        crypt.GenerateIV();
                        Iv = crypt.IV;
                    }

                    using (var cryptStream = new CryptoStream(stream, crypt.CreateEncryptor(Key, Iv), CryptoStreamMode.Write))
                    {
                        using (var writeStream = new BinaryWriter(cryptStream))
                        {
                            writeStream.Write(data);

                        }
                    }
                }

                result = stream.ToArray();

            }

            return result;

        }

        public byte[] Decode(byte[] data)
        {
            byte[] result;
          
            using (var stream = new MemoryStream(data))
            {

                using (var crypt = new RijndaelManaged())
                {

                    if (Key is null)
                    {
                        crypt.GenerateKey();
                        Key = crypt.Key;
                    }
                    if (Iv is null)
                    {
                        crypt.GenerateIV();
                        Iv = crypt.IV;
                    }

                    using (var cryptStream = new CryptoStream(stream, crypt.CreateDecryptor(Key, Iv), CryptoStreamMode.Read))
                    {
                        using (var readStream = new MemoryStream())
                        {
                            cryptStream.CopyTo(readStream);
                            result = readStream.ToArray();

                        }
                    }
                }

            }

            return result;

        }

        public void RemoveAndSetIv(ref byte[] data, int LengthIv=16)
        {
            Iv = new byte[LengthIv];
            Array.Copy(data, data.Length - Iv.Length, Iv, 0, Iv.Length);
            Array.Resize(ref data, data.Length - Iv.Length);
        }

        public void AddToDataIv(ref byte[] data)
        {
            if (Iv == null)
                throw new ArgumentException(nameof(Iv));

            Array.Resize(ref data, data.Length + Iv.Length);
            Array.Copy(Iv, 0, data, data.Length - Iv.Length, Iv.Length);
        }

    }
}
