using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Extension;
using X509Certificate = Org.BouncyCastle.X509.X509Certificate;

namespace NerdyMishka.Security.Cryptography.X509Certificates
{
    public class BouncyCastleX509CertificateBuilder : X509CertificateBuilder
    {
        private readonly X509V3CertificateGenerator v3CertGen = new X509V3CertificateGenerator();

        private readonly SecureRandom random = new SecureRandom();

        public override X509Certificate2 Build()
        {
            this.v3CertGen.Reset();

            if (this.Options is null)
                throw new InvalidOperationException("Options is null");

            if (this.Options.DistinguishedName == null)
                throw new InvalidOperationException("Missing a distinguished namae");

            var name = new X509Name(this.Options.DistinguishedName.ToString());

            var issuer = this.Options.Issuer;
            ISignatureFactory? signatureFactory = null;

            var algoOptions = this.Options.AlgorithmOptions ?? new ECDsaCertificateAlgorithmOptions();
            var hash = algoOptions.HashAlgorithmName.Name;

            X509Certificate? bouncyX509Certificate = null;

            if (issuer != null)
            {
                var bouncyX509Cert = DotNetUtilities.FromX509Certificate(issuer);               
                var privateKey = Org.BouncyCastle.Security.DotNetUtilities.GetKeyPair(issuer.PrivateKey).Private;
                var privateKeyInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(privateKey);

                this.v3CertGen.SetIssuerDN(bouncyX509Cert.SubjectDN);
                this.v3CertGen.AddExtension(X509Extensions.AuthorityKeyIdentifier, false, new AuthorityKeyIdentifierStructure(bouncyX509Cert));

                var id = privateKeyInfo.PrivateKeyAlgorithm.Algorithm.Id;
                switch (id)
                {
                    case Oids.Rsa:
                        signatureFactory = new Asn1SignatureFactory($"{hash}withRSA", privateKey, this.random);
                        break;
                    case Oids.Dsa:
                        signatureFactory = new Asn1SignatureFactory($"{hash}withDSA", privateKey, this.random);
                        break;
                    case Oids.Ec:
                        signatureFactory = new Asn1SignatureFactory($"{hash}withECDSA", privateKey, this.random);
                        break;
                    default:
                        throw new NotSupportedException($"Unknown private key algo {id}");
                }
            }

            this.v3CertGen.SetSubjectDN(name);
            this.v3CertGen.AddExtension(new KeyUsageExtension(this.Options.X509KeyUsage));
            if (this.Options.CertificateLifetime == DateRange.Zero)
                this.Options.CertificateLifetime = new DateRange(DateTimeOffset.UtcNow.AddYears(5));

            this.v3CertGen.SetNotBefore(this.Options.CertificateLifetime.Start.DateTime);
            this.v3CertGen.SetNotAfter(this.Options.CertificateLifetime.End.DateTime);

            if (this.Options.Issuer == null)
                this.v3CertGen.SetIssuerDN(name);

            if (this.Options.BasicConstraints != null)
                this.v3CertGen.AddExtension(new BasicConstraintsExtension(this.Options.BasicConstraints));

            if (this.Options.EnhancedKeyUsage != null && this.Options.EnhancedKeyUsage.Count > 0)
                this.v3CertGen.AddExtension(new EnhancedKeyUsageExtension(this.Options.EnhancedKeyUsage));

            if (this.Options.SubjectAlternativeName != null)
                this.v3CertGen.AddExtension(new SubjectAlternativeNameExtension(this.Options.SubjectAlternativeName));

            var serial = this.Options.SerialNumber;
            if (serial == System.Numerics.BigInteger.Zero)
            {
                var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                var unixTime = Convert.ToInt64((DateTime.UtcNow - epoch).TotalSeconds);
                serial = new System.Numerics.BigInteger(unixTime);
            }

            var serialNumber = new Org.BouncyCastle.Math.BigInteger(serial.ToByteArray());
            this.v3CertGen.SetSerialNumber(serialNumber);

            AsymmetricCipherKeyPair? kp = null;

            switch (algoOptions)
            {
                case ECDsaCertificateAlgorithmOptions ecdsaOptions:
                    var ecGenerator = new ECKeyPairGenerator("ECDSA");
                    var ecParams = new KeyGenerationParameters(this.random, ecdsaOptions.KeySize);
                    ecGenerator.Init(ecParams);

                    kp = ecGenerator.GenerateKeyPair();
                    signatureFactory ??= new Asn1SignatureFactory($"{hash}withECDSA", kp.Private, this.random);
                    break;

                case RSACertificateAlgorithmOptions rsaOptions:
                    var rsaGenerator = new RsaKeyPairGenerator();
                    var rsaParams = new KeyGenerationParameters(this.random, rsaOptions.KeySize);
                    rsaGenerator.Init(rsaParams);

                    kp = rsaGenerator.GenerateKeyPair();
                    signatureFactory ??= new Asn1SignatureFactory($"{hash}withRSA", kp.Private, this.random);
                    break;

                case DSACertificateAlgorithmOptions dsaOptions:
                    var dsaGenerator = new DsaKeyPairGenerator();
                    var dsaParams = new KeyGenerationParameters(this.random, dsaOptions.KeySize);

                    dsaGenerator.Init(dsaParams);
                    kp = dsaGenerator.GenerateKeyPair();
                    signatureFactory ??= new Asn1SignatureFactory($"{hash}withDSA", kp.Private, this.random);
                    break;
                default:
                    throw new NotSupportedException($"options of type {algoOptions.GetType().FullName} is not supported");
            }

            this.v3CertGen.SetPublicKey(kp.Public);

            this.v3CertGen.AddExtension(
                X509Extensions.SubjectKeyIdentifier,
                false,
                new SubjectKeyIdentifierStructure(kp.Public));

            var cert = this.v3CertGen.Generate(signatureFactory);

            cert.CheckValidity();

            if (bouncyX509Certificate != null)
                cert.Verify(bouncyX509Certificate.GetPublicKey());

            return null;
        }
    }
}
