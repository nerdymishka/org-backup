using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace NerdyMishka.Security.Cryptography.X509Certificates
{
    public class ECDsaCertificateAlgorithmOptions : X509CertificateAlgorithmOptions
    {
        private static int[] s_validSizes = new int[] { 256, 384, 521 };

        public ECDsaCertificateAlgorithmOptions()
        {
            this.KeySize = 384;
        }

        public ECDsaCertificateAlgorithmOptions(int keySize)
        {
            if (Array.IndexOf(s_validSizes, keySize) != -1)
                throw new ArgumentException($"keySize {keySize} is invalid. Valid sizes are 256, 384, 521");

            this.KeySize = keySize;
        }

        public ECDsaCertificateAlgorithmOptions(int keySize, HashAlgorithmName hashAlgorithmName)
            : this(keySize)
        {
            this.HashAlgorithmName = hashAlgorithmName;
        }

        public override AsymmetricAlgorithm Create()
        {
            var ecdsa = ECDsa.Create();
            ecdsa.KeySize = this.KeySize;
            return ecdsa;
        }
    }
}
