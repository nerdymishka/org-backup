using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NerdyMishka.Security.Cryptography.X509Certificates
{
    public class DsaCertificateOptions : CertificateAlgorithmOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DsaCertificateOptions"/> class.
        /// </summary>
        public DsaCertificateOptions()
        {
            this.KeySize = 2048;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DsaCertificateOptions"/> class.
        /// </summary>
        /// <param name="keySize">The key size.</param>
        public DsaCertificateOptions(int keySize)
        {
            if (keySize % 1024 != 0)
                throw new ArgumentException("keySize is not a valid value");

            this.KeySize = keySize;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DsaCertificateOptions"/> class.
        /// </summary>
        /// <param name="keySize">The key size.</param>
        /// <param name="hashAlgorithmName">The name of the hash algorigthm.</param>
        /// <param name="padding">The RSA signature padding.</param>
        public DsaCertificateOptions(int keySize, HashAlgorithmName hashAlgorithmName, RSASignaturePadding? padding = null)
        {
            if (keySize % 1024 != 0)
                throw new ArgumentException("keySize is not a valid value");

            this.KeySize = keySize;
            this.HashAlgorithmName = hashAlgorithmName;
        }
    }
}
