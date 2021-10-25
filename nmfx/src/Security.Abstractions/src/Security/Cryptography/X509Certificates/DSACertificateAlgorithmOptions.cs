using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace NerdyMishka.Security.Cryptography.X509Certificates
{
    public class DSACertificateAlgorithmOptions : X509CertificateAlgorithmOptions
    {
        public DSACertificateAlgorithmOptions(int keySize)
        {
            if (keySize % 1048 != 0)
                throw new ArgumentException("KeySize must a factor of 1048");

            this.KeySize = keySize;
        }

        public DSACertificateAlgorithmOptions(int keySize, HashAlgorithmName hashAlgorithmName)
            : this(keySize)
        {
            this.HashAlgorithmName = hashAlgorithmName;
        }

        public override AsymmetricAlgorithm Create()
        {
            var dsa = DSA.Create();
            dsa.KeySize = this.KeySize;
            return dsa;
        }
    }
}
