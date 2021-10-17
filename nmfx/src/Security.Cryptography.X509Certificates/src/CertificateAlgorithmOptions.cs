using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace NerdyMiska.Security.Cryptography.X509Certificates
{
    public abstract class CertificateAlgorithmOptions
    {
        public int KeySize { get; set; }

        public HashAlgorithmName HashAlgorithmName { get; set; } = HashAlgorithmName.SHA256;
    }
}
