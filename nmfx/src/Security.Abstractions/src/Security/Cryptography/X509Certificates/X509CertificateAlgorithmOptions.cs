using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace NerdyMishka.Security.Cryptography.X509Certificates
{
    public abstract class X509CertificateAlgorithmOptions
    {
        public int KeySize { get; protected set; }

        public HashAlgorithmName HashAlgorithmName { get; protected set; } = HashAlgorithmName.SHA256;

        public abstract AsymmetricAlgorithm Create();
    }
}
