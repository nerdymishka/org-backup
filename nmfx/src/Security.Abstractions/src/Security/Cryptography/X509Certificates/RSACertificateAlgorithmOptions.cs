using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace NerdyMishka.Security.Cryptography.X509Certificates
{
    public class RSACertificateAlgorithmOptions : X509CertificateAlgorithmOptions
    {
        public RSACertificateAlgorithmOptions(int keySize)
        {
            if (keySize % 1048 != 0)
                throw new ArgumentException("KeySize must a factor of 1048");

            this.KeySize = keySize;
        }

        public RSACertificateAlgorithmOptions(int keySize, HashAlgorithmName hashAlgorithmName)
            : this(keySize)
        {
            this.HashAlgorithmName = hashAlgorithmName;
        }

        public RSASignaturePadding RSASignaturePadding { get; set; } = RSASignaturePadding.Pkcs1;

        public override AsymmetricAlgorithm Create()
        {
            var rsa = RSA.Create();
            rsa.KeySize = this.KeySize;
            return rsa;
        }
    }
}
