using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace NerdyMishka.Security.Cryptography.X509Certificates
{
    public class X509CertificateBuilderOptions
    {
        public DateRange CertificateLifetime { get; set; }

        public DistinguishedName? DistinguishedName { get; set; }

        public SubjectAlternativeName? SubjectAlternativeName { get; set; }

        public HashSet<Oid>? EnhancedKeyUsage { get; set; }

        public List<IX509Extension>? Extensions { get; set; }

        public X509KeyUsageFlags X509KeyUsage { get; set; }

        public BasicConstraints? BasicConstraints { get; set; }

        public BigInteger SerialNumber { get; set; }

        public X509Certificate2? Issuer { get; set; }

        public X509CertificateAlgorithmOptions? AlgorithmOptions { get; set; }
    }
}
