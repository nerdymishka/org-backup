using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace NerdyMishka.Security.Cryptography.X509Certificates
{
    public abstract class X509CertificateBuilder
    {
       

        protected X509CertificateBuilder()
        {
            this.Options = new X509CertificateBuilderOptions();
        }

        protected X509CertificateBuilder(X509CertificateBuilderOptions options)
        {
            this.Options = options;
        }

        protected X509CertificateBuilderOptions Options { get; set; }

        public X509CertificateBuilder WithDateRange(DateRange range)
        {
            this.Options.CertificateLifetime = range;

            return this;
        }

        public X509CertificateBuilder WithDistinguishedName(DistinguishedName distinguishedName)
        {
            this.Options.DistinguishedName = distinguishedName;

            return this;
        }

        public X509CertificateBuilder WithSubjectAlternativeName(SubjectAlternativeName subjectAlternativeName)
        {
            this.Options.SubjectAlternativeName = subjectAlternativeName;

            return this;
        }

        public X509CertificateBuilder WithSerial(BigInteger serial)
        {
            this.Options.SerialNumber = serial;

            return this;
        }

        public X509CertificateBuilder WithSerial(long serial)
        {
            this.Options.SerialNumber = new BigInteger(serial);

            return this;
        }

        public X509CertificateBuilder WithSerial(byte[] bytes)
        {
            this.Options.SerialNumber = new BigInteger(bytes);

            return this;
        }

        public X509CertificateBuilder WithIssuer(X509Certificate2? x509Certificate2)
        {
            this.Options.Issuer = x509Certificate2;
            return this;
        }

        public X509CertificateBuilder WithKeyUsageFlags(X509KeyUsageFlags keyUsageFlags)
        {
            this.Options.X509KeyUsage = keyUsageFlags;

            return this;
        }

        public X509CertificateBuilder WithAlgorithmOptions(X509CertificateAlgorithmOptions options)
        {
            this.Options.AlgorithmOptions = options;

            return this;
        }

        public X509CertificateBuilder AddEnhancedKeyUsage(IEnumerable<Oid> enhancedKeyUsages)
        {
            this.Options.EnhancedKeyUsage ??= new HashSet<Oid>();
            var usages = this.Options.EnhancedKeyUsage;

            foreach (var id in enhancedKeyUsages)
            {
                if (!usages.Contains(id))
                    usages.Add(id);
            }

            return this;
        }

        public X509CertificateBuilder AddEnhancedKeyUsage(OidCollection enhancedKeyUsages)
        {
            this.Options.EnhancedKeyUsage ??= new HashSet<Oid>();
            var usages = this.Options.EnhancedKeyUsage;

            foreach (var id in enhancedKeyUsages)
            {
                if (!usages.Contains(id))
                    usages.Add(id);
            }

            return this;
        }

        public X509CertificateBuilder AddExtension(IX509Extension extension)
        {
            this.Options.Extensions ??= new List<IX509Extension>();
            this.Options.Extensions.Add(extension);

            return this;
        }

        public X509CertificateBuilder Reset()
        {
            this.Options = new X509CertificateBuilderOptions();
            return this;
        }

        public X509CertificateBuilder ResetWithOptions(X509CertificateBuilderOptions options)
        {
            this.Options = new X509CertificateBuilderOptions()
            {
                BasicConstraints = options.BasicConstraints,
                DistinguishedName = options.DistinguishedName,
                SubjectAlternativeName = options.SubjectAlternativeName,
                CertificateLifetime = options.CertificateLifetime,
                EnhancedKeyUsage = new HashSet<Oid>(options.EnhancedKeyUsage),
                AlgorithmOptions = options.AlgorithmOptions ?? new ECDsaCertificateAlgorithmOptions(384),
                Extensions = new List<IX509Extension>(options.Extensions),
                X509KeyUsage = options.X509KeyUsage,
                Issuer = options.Issuer,
            };

            return this;
        }

        public abstract X509Certificate2 Build();
    }
}
