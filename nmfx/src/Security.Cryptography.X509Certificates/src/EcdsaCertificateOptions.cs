using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace NerdyMishka.Security.Cryptography.X509Certificates
{
    /// <summary>
    /// Represents the options for an ECD signing algorithm <see cref="System.Security.Cryptography.X509Certificates.X509Certificate2"/>.
    /// </summary>
    public class EcdsaCertificateOptions : CertificateAlgorithmOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EcdsaCertificateOptions"/> class.
        /// </summary>
        public EcdsaCertificateOptions()
        {
            this.KeySize = 256;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EcdsaCertificateOptions"/> class.
        /// </summary>
        /// <param name="keySize">The key size.</param>
        /// <exception cref="ArgumentException">Thrown when the key size is not valid.</exception>
        public EcdsaCertificateOptions(int keySize)
        {
            var validSizes = new[] { 160, 224, 256, 384, 512 };
            if (Array.IndexOf(validSizes, keySize) == -1)
                throw new ArgumentException("keySize must be a valid value of 160 (legacy), 224, 256, 384, 512");

            this.KeySize = keySize;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EcdsaCertificateOptions"/> class.
        /// </summary>
        /// <param name="keySize">The key size.</param>
        /// <param name="hashAlgorithmName">The name of the hash algorithm.</param>
        /// <exception cref="ArgumentException">Thrown when the key size is not valid.</exception>
        public EcdsaCertificateOptions(int keySize, HashAlgorithmName hashAlgorithmName)
            : this(keySize)
        {
            this.HashAlgorithmName = hashAlgorithmName;
        }
    }
}
