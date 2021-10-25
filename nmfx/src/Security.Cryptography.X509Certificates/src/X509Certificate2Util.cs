using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace NerdyMishka.Security.Cryptography.X509Certificates
{
    public static class X509Certificate2Util
    {
        public static X509Certificate2 CreateChainedCert(
            X509Certificate2 signingCertificate,
            SubjectAlternativeName subjectAlternativeName,
            DistinguishedName? distinguishedName,
            BasicConstraints basicConstraints,
            OidCollection enhancedKeyUsages,
            X509KeyUsageFlags x509KeyUsageFlags,
            CertificateLifetime? certificateLifetime,
            long? serialNumber,
            AsymmetricAlgorithm asymmetricAlgorithm,
            CertificateAlgorithmOptions options)
        {
            if (!signingCertificate.HasPrivateKey)
            {
                throw new System.Security.SecurityException("signingCertificate must have a private key");
            }

            distinguishedName ??= new DistinguishedName(subjectAlternativeName);
            certificateLifetime ??= new CertificateLifetime(DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddYears(5));

            var lifetime = certificateLifetime.Value;
            var request = CreateRequest(distinguishedName, asymmetricAlgorithm, options);

            request.CertificateExtensions.Add(basicConstraints.ToX509Extension());
            request.CertificateExtensions.Add(new X509KeyUsageExtension(x509KeyUsageFlags, true));

            // set the AuthorityKeyIdentifier. There is no built-in
            // support, so it needs to be copied from the Subject Key
            // Identifier of the signing certificate and massaged slightly.
            // AuthorityKeyIdentifier is "KeyID=<subject key identifier>"
            foreach (var item in signingCertificate.Extensions)
            {
                if (item is null)
                    continue;

                if (item.Oid?.Value == Oids.SubjectKeyIdentifier)
                {
                    var issuerSubjectKey = item.RawData;

                    var segment = new ArraySegment<byte>(issuerSubjectKey, 2, issuerSubjectKey.Length - 2);
                    var authorityKeyIdentifier = new byte[segment.Count + 4];

                    // "KeyID" bytes
                    authorityKeyIdentifier[0] = 0x30;
                    authorityKeyIdentifier[1] = 0x16;
                    authorityKeyIdentifier[2] = 0x80;
                    authorityKeyIdentifier[3] = 0x14;
                    var i = 4;
                    foreach (var b in segment)
                    {
                        authorityKeyIdentifier[i] = b;
                        i++;
                    }

                    request.CertificateExtensions.Add(new X509Extension(Oids.AuthorityKeyIdentifier, authorityKeyIdentifier, false));
                    break;
                }
            }

            request.CertificateExtensions.Add(subjectAlternativeName.ToX509Extension());
            request.CertificateExtensions.Add(
                new X509EnhancedKeyUsageExtension(enhancedKeyUsages, false));
            request.CertificateExtensions.Add(
                new X509SubjectKeyIdentifierExtension(request.PublicKey, false));

            // certificate expiry: Valid from Yesterday to Now+365 days
            // Unless the signing cert's validity is less. It's not possible
            // to create a cert with longer validity than the signing cert.
            var notbefore = lifetime.NotBefore.AddDays(-1);
            if (notbefore < signingCertificate.NotBefore)
            {
                notbefore = new DateTimeOffset(signingCertificate.NotBefore);
            }

            var notafter = lifetime.NotAfter;
            if (notafter > signingCertificate.NotAfter)
            {
                notafter = new DateTimeOffset(signingCertificate.NotAfter);
            }

            if (!serialNumber.HasValue)
            {
                var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                serialNumber = Convert.ToInt64((DateTime.UtcNow - epoch).TotalSeconds);
            }

            var serial = BitConverter.GetBytes(serialNumber.Value);
            return request.Create(
                           signingCertificate,
                           notbefore,
                           notafter,
                           serial);
        }

        public static X509Certificate2 CreateSelfSignedCert(
            SubjectAlternativeName subjectAlternativeName,
            DistinguishedName? distinguishedName,
            BasicConstraints basicConstraints,
            OidCollection enhancedKeyUsages,
            X509KeyUsageFlags x509KeyUsageFlags,
            CertificateLifetime? certificateLifetime,
            long? serialNumber,
            AsymmetricAlgorithm asymmetricAlgorithm,
            CertificateAlgorithmOptions options)
        {
            var lifetime = certificateLifetime ?? CertificateLifetime.AsTenYears();
            distinguishedName ??= new DistinguishedName(subjectAlternativeName);

            var request = CreateRequest(
                basicConstraints,
                subjectAlternativeName,
                enhancedKeyUsages,
                x509KeyUsageFlags,
                asymmetricAlgorithm,
                options,
                distinguishedName);

            if (!serialNumber.HasValue)
            {
                var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                serialNumber = Convert.ToInt64((DateTime.UtcNow - epoch).TotalSeconds);
            }

            var serial = BitConverter.GetBytes(serialNumber.Value);

            var name = distinguishedName.ToX500DistinguishedName();
            var signatureGenerator = GetSignatureGenerator(asymmetricAlgorithm, options);

            return request.Create(
                name,
                signatureGenerator,
                lifetime.NotBefore,
                lifetime.NotAfter,
                serial);
        }

        public static CertificateRequest CreateRequest(
            BasicConstraints basicConstraints,
            SubjectAlternativeName subjectAlternativeName,
            OidCollection enhancedKeyUsages,
            X509KeyUsageFlags x509KeyUsageFlags,
            AsymmetricAlgorithm asymmetricAlgorithm,
            CertificateAlgorithmOptions options,
            DistinguishedName? distinguishedName = null)
        {
            distinguishedName ??= new DistinguishedName(subjectAlternativeName);
            var request = CreateRequest(distinguishedName, asymmetricAlgorithm, options);
            request.CertificateExtensions.Add(basicConstraints.ToX509Extension());
            request.CertificateExtensions.Add(new X509KeyUsageExtension(x509KeyUsageFlags, true));
            request.CertificateExtensions.Add(subjectAlternativeName.ToX509Extension());
            request.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension(enhancedKeyUsages, false));
            request.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(request.PublicKey, false));

            return request;
        }

        public static CertificateRequest CreateRequest(
            DistinguishedName distinguishedName,
            AsymmetricAlgorithm asymmetricAlgorithm,
            CertificateAlgorithmOptions options)
        {
            switch (asymmetricAlgorithm)
            {
                case ECDsa ecdsa:
                    return new CertificateRequest(
                       distinguishedName.ToX500DistinguishedName(),
                       ecdsa,
                       options.HashAlgorithmName);

                case DSA dsa:
                    X509SignatureGenerator dsaGen = new DSAX509SignatureGenerator(dsa);
                    return new CertificateRequest(
                        distinguishedName.ToX500DistinguishedName(),
                        dsaGen.PublicKey,
                        options.HashAlgorithmName);

                case RSA rsa:
                    if (options is RsaCertificateOptions rsaOptions)
                    {
                       return new CertificateRequest(
                           distinguishedName.ToX500DistinguishedName(),
                           rsa,
                           options.HashAlgorithmName,
                           rsaOptions.RSASignaturePadding);
                    }
                    else
                    {
                       return new CertificateRequest(
                          distinguishedName.ToX500DistinguishedName(),
                          rsa,
                          options.HashAlgorithmName,
                          RSASignaturePadding.Pkcs1);
                    }

                default:
                    throw new NotSupportedException(asymmetricAlgorithm.GetType().Name + " is not supported");
            }
        }

        private static X509SignatureGenerator GetSignatureGenerator(AsymmetricAlgorithm asymmetricAlgorithm, CertificateAlgorithmOptions options)
        {
            switch (asymmetricAlgorithm)
            {
                case ECDsa ecdsa:
                    return X509SignatureGenerator.CreateForECDsa(ecdsa);

                case DSA dsa:
                    return new DSAX509SignatureGenerator(dsa);

                case RSA rsa:
                    if (options is RsaCertificateOptions rsaOptions)
                    {
                        return X509SignatureGenerator.CreateForRSA(rsa, rsaOptions.RSASignaturePadding);
                    }
                    else
                    {
                        return X509SignatureGenerator.CreateForRSA(rsa, RSASignaturePadding.Pkcs1);
                    }

                default:
                    throw new NotSupportedException(asymmetricAlgorithm.GetType().Name + " is not supported");
            }
        }
    }
}
