using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace NerdyMishka.Security.Cryptography.X509Certificates
{
    public class X509Certificate2Factory : IDisposable
    {
        private bool externallyControlled;

        public X509Certificate2Factory(CertificateAlgorithmOptions? options = null)
        {
            options ??= new RsaCertificateOptions();
            this.UseOptions(options);
            this.AsymmetricAlgorithm ??= RSA.Create(2048);
            this.Options ??= new RsaCertificateOptions();
        }

        protected virtual AsymmetricAlgorithm AsymmetricAlgorithm { get; private set; }

        protected virtual CertificateAlgorithmOptions Options { get; private set; }

        public X509Certificate2 CreateChainedCert(
           X509Certificate2 signingCertificate,
           SubjectAlternativeName subjectAlternativeName,
           BasicConstraints basicConstraints,
           OidCollection enhancedKeyUsages,
           X509KeyUsageFlags x509KeyUsageFlags,
           CertificateLifetime? certificateLifetime = null)
        {
            return X509Certificate2Util.CreateChainedCert(
                signingCertificate,
                subjectAlternativeName,
                null,
                basicConstraints,
                enhancedKeyUsages,
                x509KeyUsageFlags,
                certificateLifetime,
                null,
                this.AsymmetricAlgorithm,
                this.Options);
        }

        public X509Certificate2 CreateChainedCert(
           X509Certificate2 signingCertificate,
           SubjectAlternativeName subjectAlternativeName,
           DistinguishedName? distinguishedName,
           BasicConstraints basicConstraints,
           OidCollection enhancedKeyUsages,
           X509KeyUsageFlags x509KeyUsageFlags,
           CertificateLifetime? certificateLifetime = null)
        {
            return X509Certificate2Util.CreateChainedCert(
                signingCertificate,
                subjectAlternativeName,
                distinguishedName,
                basicConstraints,
                enhancedKeyUsages,
                x509KeyUsageFlags,
                certificateLifetime,
                null,
                this.AsymmetricAlgorithm,
                this.Options);
        }

        public X509Certificate2 CreateSelfSignedCert(
            SubjectAlternativeName subjectAlternativeName,
            BasicConstraints basicConstraints,
            OidCollection enhancedKeyUsages,
            X509KeyUsageFlags x509KeyUsageFlags,
            CertificateLifetime certificateLifetime)
        {
            return X509Certificate2Util.CreateSelfSignedCert(
                subjectAlternativeName,
                null,
                basicConstraints,
                enhancedKeyUsages,
                x509KeyUsageFlags,
                certificateLifetime,
                null,
                this.AsymmetricAlgorithm,
                this.Options);
        }

        public X509Certificate2 CreateSelfSignedCert(
            SubjectAlternativeName subjectAlternativeName,
            DistinguishedName distinguishedName,
            BasicConstraints basicConstraints,
            OidCollection enhancedKeyUsages,
            X509KeyUsageFlags x509KeyUsageFlags,
            CertificateLifetime certificateLifetime)
        {
            return X509Certificate2Util.CreateSelfSignedCert(
                subjectAlternativeName,
                distinguishedName,
                basicConstraints,
                enhancedKeyUsages,
                x509KeyUsageFlags,
                certificateLifetime,
                null,
                this.AsymmetricAlgorithm,
                this.Options);
        }

        public CertificateRequest CreateRequest(
           SubjectAlternativeName subjectAlternativeName,
           BasicConstraints basicConstraints,
           OidCollection enhancedKeyUsages,
           X509KeyUsageFlags x509KeyUsageFlags)
        {
            return X509Certificate2Util.CreateRequest(
                basicConstraints,
                subjectAlternativeName,
                enhancedKeyUsages,
                x509KeyUsageFlags,
                this.AsymmetricAlgorithm,
                this.Options,
                null);
        }

        public CertificateRequest CreateRequest(
            SubjectAlternativeName subjectAlternativeName,
            DistinguishedName distinguishedName,
            BasicConstraints basicConstraints,
            OidCollection enhancedKeyUsages,
            X509KeyUsageFlags x509KeyUsageFlags)
        {
            return X509Certificate2Util.CreateRequest(
                basicConstraints,
                subjectAlternativeName,
                enhancedKeyUsages,
                x509KeyUsageFlags,
                this.AsymmetricAlgorithm,
                this.Options,
                distinguishedName);
        }

        public CertificateRequest CreateRequest(
            DistinguishedName distinguishedName)
        {
            return X509Certificate2Util.CreateRequest(
                distinguishedName,
                this.AsymmetricAlgorithm,
                this.Options);
        }

        public void UseOptions(CertificateAlgorithmOptions options, AsymmetricAlgorithm algo)
        {
            if (this.externallyControlled && this.AsymmetricAlgorithm != null)
            {
                this.AsymmetricAlgorithm.Dispose();
                this.externallyControlled = false;
            }

            this.externallyControlled = true;

            switch (options)
            {
                case RsaCertificateOptions rsaOptions:
                    if (algo is RSA rsa)
                    {
                       this.AsymmetricAlgorithm = rsa;
                       this.Options = options;
                       return;
                    }

                    throw new NotSupportedException($"algo type {algo.GetType().FullName} must match options type ({options.GetType().FullName})");

                case EcdsaCertificateOptions ecdsaOptions:
                    if (algo is ECDsa ecdsa)
                    {
                       this.AsymmetricAlgorithm = ecdsa;
                       this.Options = options;
                       return;
                    }

                    throw new NotSupportedException($"algo type {algo.GetType().FullName} must match options type ({options.GetType().FullName})");

                default:
                    throw new NotSupportedException($"options of type {options.GetType().FullName} is not supported");
            }
        }

        public void UseOptions(CertificateAlgorithmOptions options)
        {
            if (this.externallyControlled && this.AsymmetricAlgorithm != null)
            {
                this.AsymmetricAlgorithm.Dispose();
                this.externallyControlled = false;
            }

            switch (options)
            {
                case RsaCertificateOptions rsaOptions:
                    this.AsymmetricAlgorithm = RSA.Create(rsaOptions.KeySize);
                    this.Options = options;
                    break;
                case EcdsaCertificateOptions ecdsaOptions:
                    this.AsymmetricAlgorithm = ECDsa.Create("ECDsa") ?? throw new NullReferenceException("ECDsa did not return from ECDsa.Create()");
                    this.Options = options;
                    break;

                default:
                    throw new NotSupportedException($"options of type {options.GetType().FullName} is not supported");
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            if (!this.externallyControlled)
                this.AsymmetricAlgorithm?.Dispose();
        }
    }
}
