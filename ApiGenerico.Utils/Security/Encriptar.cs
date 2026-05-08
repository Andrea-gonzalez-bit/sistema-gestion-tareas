using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ApiGenerico.Utils.Security
{
    public static class Encriptar_alt
    {
        //static string _key = "3EROC2BEW1ALOSNOC0YRTNES";
        static string _key = "ADNKey_2005_28_05";
        static string _vec = "ADN_Vector";
        public static string Encrypt(string texto)
        {
            var key = Encoding.UTF8.GetBytes(_key);

            using (var aesAlg = Aes.Create())
            {
                //aesAlg.IV
                aesAlg.IV = Encoding.UTF8.GetBytes(_vec);
                using (var encryptor = aesAlg.CreateEncryptor(key, aesAlg.IV))
                {
                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(texto);
                        }

                        var iv = aesAlg.IV;

                        var decryptedContent = msEncrypt.ToArray();

                        var result = new byte[iv.Length + decryptedContent.Length];

                        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                        Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

                        return Convert.ToBase64String(result);
                    }
                }
            }
        }

        public static string Decrypt(string textoCifrado)
        {
            var fullCipher = Convert.FromBase64String(textoCifrado);

            var iv = new byte[16];
            var cipher = new byte[16];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, iv.Length);
            var key = Encoding.UTF8.GetBytes(_key);

            using (var aesAlg = Aes.Create())
            {
                //aesAlg.IV
                aesAlg.IV = Encoding.UTF8.GetBytes(_vec);

                using (var decryptor = aesAlg.CreateDecryptor(key, iv))
                {
                    string result;
                    using (var msDecrypt = new MemoryStream(cipher))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                result = srDecrypt.ReadToEnd();
                            }
                        }
                    }

                    return result;
                }
            }
        }
    }

    public class Encriptar
    {
        private string stringIV = "ADN_Vector";
        private string stringKey = "ADNKey_2005_28_05";

        public string CifrarCadena(string CadenaOriginal)
        {
            MemoryStream stream = null;
            try
            {
                if ((this.stringKey == null) || (this.stringIV == null))
                {
                    throw new Exception("Error al inicializar la clave y el vector");
                }
                byte[] key = this.GenerarArregloKeyByteArray();
                byte[] iV = this.GenerarVectorIVByteArray();
                byte[] bytes = Encoding.UTF8.GetBytes(CadenaOriginal);
                stream = new MemoryStream(CadenaOriginal.Length * 2);
                ICryptoTransform transform = this.Criptografic(Accion.ACC_ENCRIPTAR, key, iV);
                CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Write);
                stream2.Write(bytes, 0, bytes.Length);
                stream2.Close();
            }
            catch
            {
                throw;
            }
            return Convert.ToBase64String(stream.ToArray());
        }

        public ICryptoTransform Criptografic(Accion acc, byte[] Key, byte[] IV)
        {
            TripleDESCryptoServiceProvider provider = new TripleDESCryptoServiceProvider();
            switch (acc)
            {
                case Accion.ACC_ENCRIPTAR:
                    return provider.CreateEncryptor(Key, IV);

                case Accion.ACC_DESENCRIPTAR:
                    return provider.CreateDecryptor(Key, IV);
            }
            return null;
        }

        public string DescifrarCadena(string CadenaCifrada)
        {
            MemoryStream stream = null;
            try
            {
                if ((this.stringKey == null) || (this.stringIV == null))
                {
                    throw new Exception("Error al inicializar la clave y el vector.");
                }
                byte[] key = this.GenerarArregloKeyByteArray();
                byte[] iV = this.GenerarVectorIVByteArray();
                byte[] buffer = Convert.FromBase64String(CadenaCifrada);
                stream = new MemoryStream(CadenaCifrada.Length);
                ICryptoTransform transform = this.Criptografic(Accion.ACC_DESENCRIPTAR, key, iV);
                CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Write);
                stream2.Write(buffer, 0, buffer.Length);
                stream2.Close();
            }
            catch
            {
                throw;
            }
            return Encoding.UTF8.GetString(stream.ToArray());
        }

        private byte[] GenerarArregloKeyByteArray()
        {
            if (this.stringKey.Length < 0x10)
            {
                this.stringKey = this.stringKey.PadRight(0x10);
            }
            else
            {
                this.stringKey = this.stringKey.Substring(0, 0x10);
            }
            return Encoding.UTF8.GetBytes(this.stringKey);
        }

        private byte[] GenerarVectorIVByteArray()
        {
            if (this.stringIV.Length < 8)
            {
                this.stringIV = this.stringIV.PadRight(8);
            }
            else
            {
                this.stringIV = this.stringIV.Substring(0, 8);
            }
            return Encoding.UTF8.GetBytes(this.stringIV);
        }

        public enum Accion
        {
            ACC_ENCRIPTAR,
            ACC_DESENCRIPTAR
        }
    }

    public class EncriptarVC
    {

        private string hash = "VISIONCENTER";
        private string key = "VISIONCENTER2016";

        private string StringIV
        {
            get
            {
                return hash;
            }
            set
            {
                hash = value;
            }
        }

        private string StringKey
        {
            get
            {
                return key;
            }
            set
            {
                key = value;
            }
        }

        public string CifrarCadena(string CadenaOriginal)
        {
            if (CadenaOriginal != null)
            {
                MemoryStream stream = null;
                try
                {
                    if ((this.StringKey == null) || (this.StringIV == null))
                    {
                        throw new Exception("Error al inicializar la clave y el vector");
                    }
                    byte[] key = this.GenerarArregloKeyByteArray();
                    byte[] iV = this.GenerarVectorIVByteArray();
                    byte[] bytes = Encoding.UTF8.GetBytes(CadenaOriginal);
                    stream = new MemoryStream(CadenaOriginal.Length * 2);
                    ICryptoTransform transform = this.Criptografic(Accion.ACC_ENCRIPTAR, key, iV);
                    CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Write);
                    stream2.Write(bytes, 0, bytes.Length);
                    stream2.Close();
                }
                catch
                {
                    throw;
                }
                return Convert.ToBase64String(stream.ToArray());
            }

            return null;
        }

        public ICryptoTransform Criptografic(Accion acc, byte[] Key, byte[] IV)
        {
            TripleDESCryptoServiceProvider provider = new TripleDESCryptoServiceProvider();
            switch (acc)
            {
                case Accion.ACC_ENCRIPTAR:
                    return provider.CreateEncryptor(Key, IV);

                case Accion.ACC_DESENCRIPTAR:
                    return provider.CreateDecryptor(Key, IV);
            }
            return null;
        }

        public string DescifrarCadena(string CadenaCifrada)
        {
            MemoryStream stream = null;
            try
            {
                if ((this.StringKey == null) || (this.StringIV == null))
                {
                    throw new Exception("Error al inicializar la clave y el vector.");
                }
                byte[] key = this.GenerarArregloKeyByteArray();
                byte[] iV = this.GenerarVectorIVByteArray();
                byte[] buffer = Convert.FromBase64String(CadenaCifrada);
                stream = new MemoryStream(CadenaCifrada.Length);
                ICryptoTransform transform = this.Criptografic(Accion.ACC_DESENCRIPTAR, key, iV);
                CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Write);
                stream2.Write(buffer, 0, buffer.Length);
                stream2.Close();
            }
            catch (Exception e)
            {
                return CadenaCifrada;
            }
            return Encoding.UTF8.GetString(stream.ToArray());
        }

        private byte[] GenerarArregloKeyByteArray()
        {
            if (this.StringKey.Length < 0x10)
            {
                this.StringKey = this.StringKey.PadRight(0x10);
            }
            else
            {
                this.StringKey = this.StringKey.Substring(0, 0x10);
            }
            return Encoding.UTF8.GetBytes(this.StringKey);
        }

        private byte[] GenerarVectorIVByteArray()
        {
            if (this.StringIV.Length < 8)
            {
                this.StringIV = this.StringIV.PadRight(8);
            }
            else
            {
                this.StringIV = this.StringIV.Substring(0, 8);
            }
            return Encoding.UTF8.GetBytes(this.StringIV);
        }

        public enum Accion
        {
            ACC_ENCRIPTAR,
            ACC_DESENCRIPTAR
        }

    }
}
