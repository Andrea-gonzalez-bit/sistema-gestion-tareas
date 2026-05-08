using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ApiGenerico.Utils.Security
{
    public class EncryptionService : IEncryptionService
    {
        private string StringIV = "ADN_Vector";
        private string StringKey = "ADNKey_2005_28_05";

        public string GetSha256(string strSha256)
        {
            SHA256 sha256 = SHA256Managed.Create();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] stream = null;
            StringBuilder sb = new StringBuilder();
            stream = sha256.ComputeHash(encoding.GetBytes(strSha256));
            for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
            return sb.ToString();
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

        /// <summary>
        /// Encripta una cadena
        /// </summary>
        /// <param name="CadenaOriginal"></param>
        /// <returns></returns>

        public string Encrypt(string OriginalSting)
        {
            if (OriginalSting != null)
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
                    byte[] bytes = Encoding.UTF8.GetBytes(OriginalSting);
                    stream = new MemoryStream(OriginalSting.Length * 2);
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

        /// <summary>
        /// Desencripta una cade de Byte.
        /// </summary>
        /// <param name="CadenaCifrada"></param>
        /// <returns></returns>
        public string Decrypt(string EncryptedString)
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
                byte[] buffer = Convert.FromBase64String(EncryptedString);
                stream = new MemoryStream(EncryptedString.Length);
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
