using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace NerdyMiska.Security.Cryptography.X509Certificates
{
    public class EcdsaCertificateOptions : CertificateAlgorithmOptions
    {
        public EcdsaCertificateOptions()
        {
            this.KeySize = 256;
        }

        public EcdsaCertificateOptions(int keySize)
        {
            var validSizes = new[] { 160, 224, 256, 384, 512 };
            if (Array.IndexOf(validSizes, keySize) == -1)
                throw new ArgumentException("keySize must be a valid value of 160 (legacy), 224, 256, 384, 512");

            this.KeySize = keySize;
        }

        public EcdsaCertificateOptions(int keySize, HashAlgorithmName hashAlgorithmName)
            : this(keySize)
        {
            this.HashAlgorithmName = hashAlgorithmName;
        }
    }
}
