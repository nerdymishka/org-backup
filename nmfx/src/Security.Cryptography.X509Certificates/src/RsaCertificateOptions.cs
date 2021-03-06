using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace NerdyMishka.Security.Cryptography.X509Certificates
{
    /// <summary>
    /// Represents the options for an RSA <see cref="System.Security.Cryptography.X509Certificates.X509Certificate2"/>.
    /// </summary>
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
        /// <param name="keySize">The key size.</param>
        public RsaCertificateOptions(int keySize)
        {
            if (keySize % 1024 != 0)
                throw new ArgumentException("keySize is not a valid value");

            this.KeySize = keySize;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RsaCertificateOptions"/> class.
        /// </summary>
        /// <param name="keySize">The key size.</param>
        /// <param name="hashAlgorithmName">The name of the hash algorigthm.</param>
        /// <param name="padding">The RSA signature padding.</param>
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
