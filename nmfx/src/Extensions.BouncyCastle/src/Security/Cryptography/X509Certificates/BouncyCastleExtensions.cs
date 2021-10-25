using System;
using System.Collections.Generic;
using System.Text;
using Org.BouncyCastle.X509;

namespace NerdyMishka.Security.Cryptography.X509Certificates
{
    public static class BouncyCastleExtensions
    {
        public static X509V3CertificateGenerator AddExtension(this X509V3CertificateGenerator generator, IX509Extension extension)
        {
            generator.AddExtension(extension.ObjectIdentifier, extension.IsCritical, extension.Data);
            return generator;
        }
    }
}
