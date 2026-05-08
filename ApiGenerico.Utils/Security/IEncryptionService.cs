using System;
using System.Collections.Generic;
using System.Text;

namespace ApiGenerico.Utils.Security
{
    public interface IEncryptionService
    {
        public string GetSha256(string strSha256);
        public string Encrypt(string OriginalSting);
        public string Decrypt(string EncryptedString);
    }
}
