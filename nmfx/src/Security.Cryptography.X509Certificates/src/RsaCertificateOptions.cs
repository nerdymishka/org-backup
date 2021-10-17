using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace NerdyMiska.Security.Cryptography.X509Certificates
{
    public class RsaCertificateOptions : CertificateAlgorithmOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RsaCertificateOptions"/> class.
        /// </summary>
        public RsaCertificateOptions()
        {
            this.KeySize = 2048;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RsaCertificateOptions"/> class.
        /// </summary>
        /// <param name="keySize">The key size</param>
        public RsaCertificateOptions(int keySize)
        {
            if (keySize % 1024 != 0)
                throw new ArgumentException("keySize is not a valid value");

            this.KeySize = keySize;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RsaCertificateOptions"/> class.
        /// </summary>
        /// <param name="keySize"></param>
        /// <param name="hashAlgorithmName"></param>
        /// <param name="padding"></param>
        public RsaCertificateOptions(int keySize, HashAlgorithmName hashAlgorithmName, RSASignaturePadding? padding = null)
        {
            if (keySize % 1024 != 0)
                throw new ArgumentException("keySize is not a valid value");

            this.KeySize = keySize;
            this.HashAlgorithmName = hashAlgorithmName;
            this.RSASignaturePadding = padding ?? RSASignaturePadding.Pkcs1;
        }

        /// <summary>
        /// Gets or sets the RSA signature padding.
        /// </summary>
        public RSASignaturePadding RSASignaturePadding { get; set; } = RSASignaturePadding.Pkcs1;
    }
}
