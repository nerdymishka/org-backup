using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Extension;

namespace NerdyMishka.Security.Cryptography.X509Certificates
{
    public static class BouncyX509CertificateUtil
    {
        private static readonly X509V3CertificateGenerator v3CertGen = new X509V3CertificateGenerator();

        public static void WritePublicKey(this PemWriter writer, AsymmetricKeyParameter publicKey)
        {
            var publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicKey);
            var pemObject = new Org.BouncyCastle.Utilities.IO.Pem.PemObject("PUBLIC KEY", publicKeyInfo.GetEncoded());
            writer.WriteObject(pemObject);
        }

        public static void WritePublicKeyAsPem(this TextWriter writer, AsymmetricKeyParameter publicKey)
        {
            var pemWriter = new PemWriter(writer);
            pemWriter.WritePublicKey(publicKey);
        }

        public static void WritePublicKeyAsPem(this Stream stream, AsymmetricKeyParameter publicKey)
        {
            using var sw = new StreamWriter(stream);
            var pemWriter = new PemWriter(sw);
            pemWriter.WritePublicKey(publicKey);
        }

        public static string WritePublicKeyAsPemString(this AsymmetricKeyParameter publicKey)
        {
            using var sw = new StringWriter();
            var pemWriter = new PemWriter(sw);
            pemWriter.WritePublicKey(publicKey);
            return sw.ToString();
        }

        public static string WritePrivateKeyAsPemString(this AsymmetricKeyParameter privateKey, IPasswordFinder? finder = null)
        {
            using var sw = new StringWriter();
            var pemWriter = new PemWriter(sw);
            pemWriter.WritePrivateKey(privateKey, finder);
            return sw.ToString();
        }

        public static void WritePublicKeyPem(this TextWriter writer, AsymmetricKeyParameter publicKey)
        {
            var publicKeyInfo = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(publicKey);
            var pemWriter = new PemWriter(writer);
            var pemObject = new Org.BouncyCastle.Utilities.IO.Pem.PemObject("PUBLIC KEY", publicKeyInfo.GetEncoded());
            pemWriter.WriteObject(pemObject);
        }

        public static void WritePrivateKey(this PemWriter writer, AsymmetricKeyParameter privateKey, IPasswordFinder? finder = null)
        {
            var pkcsgen = new Pkcs8Generator(privateKey);

            if (finder != null)
                pkcsgen.Password = finder.GetPassword();

            writer.WriteObject(pkcsgen.Generate());
        }

        public static void WritePrivateKeyAsPem(this TextWriter writer, AsymmetricKeyParameter privateKey, IPasswordFinder? finder = null)
        {
            var pemWriter = new PemWriter(writer);
            pemWriter.WritePrivateKey(privateKey, finder);
        }

        public static void WritePrivateKeyPem(this Stream stream, AsymmetricKeyParameter privateKey, IPasswordFinder? finder = null)
        {
            using (var streamWriter = new StreamWriter(stream))
            {
                WritePrivateKeyAsPem(streamWriter, privateKey);
            }
        }

        public static AsymmetricKeyParameter? ReadPemAsPublicKey(TextReader reader, IPasswordFinder? finder)
        {
            var pemReader = finder == null ? new PemReader(reader) : new PemReader(reader, finder);
            Org.BouncyCastle.Utilities.IO.Pem.PemObject? o = null;
            Org.BouncyCastle.Utilities.IO.Pem.PemObject? last = null;
            while ((o = pemReader.ReadPemObject()) != null)
            {
                if (!o.Type.EndsWith("PRIVATE KEY", StringComparison.OrdinalIgnoreCase))
                {
                    last = o;
                }
            }

            if (last == null)
                return null;

            return PublicKeyFactory.CreateKey(last.Content);
        }

        public static AsymmetricKeyParameter? ReadPemAsPrivateKey(TextReader reader, IPasswordFinder? finder)
        {
            var pemReader = finder == null ? new PemReader(reader) : new PemReader(reader, finder);
            Org.BouncyCastle.Utilities.IO.Pem.PemObject? o = null;
            while ((o = pemReader.ReadPemObject()) != null)
            {
                if (o.Type.EndsWith("PRIVATE KEY", StringComparison.OrdinalIgnoreCase))
                {
                    return PrivateKeyFactory.CreateKey(o.Content);
                }
            }

            return null;
        }

        public static X509Certificate2 ReadPemAsX509Certificate2(string pem, IPasswordFinder? finder, SecureRandom? random)
        {
            var bytes = ReadPemAsBytes(pem, finder, random).ToArray();
            SecureString? ss = null;
            if (finder != null)
            {
                ss = new SecureString();
                var pw = finder.GetPassword();
                foreach (var c in pw)
                    ss.AppendChar(c);
            }

            return new X509Certificate2(bytes, ss, X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet);
        }

        public static X509Certificate2 ReadPemAsX509Certificate2(TextReader reader, IPasswordFinder? finder, SecureRandom? random)
        {
            var bytes = ReadPemAsBytes(reader, finder, random).ToArray();
            SecureString? ss = null;
            if (finder != null)
            {
                ss = new SecureString();
                var pw = finder.GetPassword();
                foreach (var c in pw)
                    ss.AppendChar(c);
            }

            return new X509Certificate2(bytes, ss, X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet);
        }

        public static ReadOnlySpan<byte> ReadPemAsBytes(string pem, IPasswordFinder? finder, SecureRandom? random = null)
        {
            using var ms = new MemoryStream();
            ReadPemToStream(pem, ms, finder, random);
            ms.Flush();
            return ms.ToArray();
        }

        public static ReadOnlySpan<byte> ReadPemAsBytes(TextReader reader, IPasswordFinder? finder, SecureRandom? random = null)
        {
            using var ms = new MemoryStream();
            ReadPemToStream(reader, ms, finder, random);
            ms.Flush();
            return ms.ToArray();
        }

        public static void ReadPemToStream(string pem, Stream stream, IPasswordFinder? finder = null, SecureRandom? random = null)
        {
            using var reader = new StringReader(pem);
            random ??= new SecureRandom();
            var store = ReadPem(reader, finder);
            finder ??= new PasswordStore();

            store.Save(stream, finder.GetPassword(), random);
        }

        public static void ReadPemToStream(TextReader reader, Stream stream, IPasswordFinder? finder = null, SecureRandom? random = null)
        {
            random ??= new SecureRandom();
            var store = ReadPem(reader, finder);

            finder ??= new PasswordStore();
            store.Save(stream, finder.GetPassword(), random);
        }

        public static Pkcs12Store ReadPem(string pem, IPasswordFinder? finder = null)
        {
            using var reader = new StringReader(pem);
            return ReadPem(new StringReader(pem), finder);
        }

        public static Pkcs12Store ReadPem(TextReader reader, IPasswordFinder? finder = null)
        {
            var store = new Pkcs12StoreBuilder().Build();
            store.ReadPem(reader, "0", finder);
            return store;
        }

        public static void ReadPem(this Pkcs12Store store, TextReader reader, string? entryName = null, IPasswordFinder? finder = null)
        {
            entryName = entryName ?? "0";
            var pemReader = finder == null ? new PemReader(reader) : new PemReader(reader, finder);
            var chain = new List<X509CertificateEntry>();

            object? o = null;
            AsymmetricKeyEntry? privateKey = null;
            while ((o = pemReader.ReadObject()) != null)
            {
                switch (o)
                {
                    case Org.BouncyCastle.X509.X509Certificate cert:
                        chain.Add(new X509CertificateEntry(cert));
                        break;
                    case AsymmetricCipherKeyPair keypair:
                        privateKey = new AsymmetricKeyEntry(keypair.Private);
                        break;
                }
            }

            store.SetKeyEntry(entryName, privateKey, chain.ToArray());
        }

        public static X509Certificate2 CreateChainCert(
            X509Certificate2 issuer,
            SubjectAlternativeName subjectAlternativeName,
            DistinguishedName? distinguishedName,
            BasicConstraints basicConstraints,
            OidCollection enhancedKeyUsages,
            X509KeyUsageFlags x509KeyUsageFlags,
            CertificateLifetime? certificateLifetime,
            long? serialNumber,
            CertificateAlgorithmOptions options)
        {
            if (!issuer.HasPrivateKey)
                throw new ArgumentException("private key from issuer is missing", nameof(issuer));

            var bouncyX509Cert = DotNetUtilities.FromX509Certificate(issuer);

            distinguishedName ??= new DistinguishedName(subjectAlternativeName);
            ISignatureFactory? signatureFactory = null;
            var privateKey = Org.BouncyCastle.Security.DotNetUtilities.GetKeyPair(issuer.PrivateKey).Private;



            if (issuer.GetECDsaPrivateKey() != null)
            {
                signatureFactory = new Asn1SignatureFactory("SHA256wtihECSDA", privateKey);
            }
            else if (issuer.GetRSAPrivateKey() != null)
            {
                signatureFactory = new Asn1SignatureFactory("SHA256withRSA", privateKey);
            }
            else if (issuer.GetDSAPrivateKey() != null)
            {
                signatureFactory = new Asn1SignatureFactory("SHA256withDSA", privateKey);
            }
            else
            {
                throw new NotSupportedException("private key for issuer was not RSA, DSA, or ECDsa");
            }

            var lifetime = certificateLifetime ?? CertificateLifetime.AsFiveYears();
            if (!serialNumber.HasValue)
            {
                var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                var unixTime = Convert.ToInt64((DateTime.UtcNow - epoch).TotalSeconds);
                serialNumber = unixTime;
            }

            var serial = BigInteger.ValueOf(serialNumber.Value);
            var random = new SecureRandom();

            v3CertGen.Reset();
            var subject = new X509Name(distinguishedName.ToString());
            v3CertGen.SetIssuerDN(PrincipalUtilities.GetSubjectX509Principal(bouncyX509Cert));
            v3CertGen.SetNotBefore(lifetime.NotBefore.UtcDateTime);
            v3CertGen.SetNotAfter(lifetime.NotAfter.UtcDateTime);
            v3CertGen.SetSerialNumber(serial);
            v3CertGen.AddExtension(X509Extensions.BasicConstraints, basicConstraints.Critical, basicConstraints.ToBouncyX509Extension());
            v3CertGen.AddExtension(X509Extensions.SubjectAlternativeName, true, subjectAlternativeName.ToBouncyX509Extension());
            v3CertGen.AddExtension(X509Extensions.KeyUsage, true, x509KeyUsageFlags.ToX509Extension());
            v3CertGen.AddExtension(X509Extensions.ExtendedKeyUsage, true, enhancedKeyUsages.ToX509Extension());
            v3CertGen.SetSubjectDN(subject);
            AsymmetricCipherKeyPair? kp = null;

            if (options is RsaCertificateOptions rsaOptions)
            {
                var rsaGenerator = new RsaKeyPairGenerator();
                var rsaParams = new KeyGenerationParameters(random, rsaOptions.KeySize);
                rsaGenerator.Init(rsaParams);

                kp = rsaGenerator.GenerateKeyPair();
            }
            else if (options is EcdsaCertificateOptions ecdsaOptions)
            {
                var ecGenerator = new ECKeyPairGenerator("ECDSA");
                var ecParams = new KeyGenerationParameters(random, ecdsaOptions.KeySize);
                ecGenerator.Init(ecParams);

                kp = ecGenerator.GenerateKeyPair();
            }
            else
            {
                throw new NotSupportedException($"{options.GetType().FullName} is not supported");
            }

            v3CertGen.AddExtension(
                X509Extensions.SubjectKeyIdentifier,
                false,
                new SubjectKeyIdentifierStructure(kp.Public));

            v3CertGen.AddExtension(
                X509Extensions.AuthorityKeyIdentifier,
                false,
                new AuthorityKeyIdentifierStructure(bouncyX509Cert));

            IDictionary bagAttr = new Hashtable();

            bagAttr.Add(
                PkcsObjectIdentifiers.Pkcs9AtFriendlyName.Id,
                new DerBmpString(distinguishedName.FriendlyName ?? distinguishedName.CommonName));

            var cert = v3CertGen.Generate(signatureFactory);
            cert.CheckValidity(DateTime.UtcNow);
            cert.Verify(bouncyX509Cert.GetPublicKey());

            string alias = cert.SubjectDN.ToString();
            Pkcs12Store store = new Pkcs12StoreBuilder().Build();

            X509CertificateEntry certEntry = new X509CertificateEntry(cert);
            store.SetCertificateEntry(alias, certEntry);

            // TODO: This needs extra logic to support a certificate chain
            AsymmetricKeyEntry keyEntry = new AsymmetricKeyEntry(kp.Private);
            store.SetKeyEntry(alias, keyEntry, new X509CertificateEntry[] { certEntry });

            byte[] certificateData;
            var tmpPassword = "temp@#23d23421ds_+";
            using (var ms = new MemoryStream())
            {
                store.Save(ms, tmpPassword.ToCharArray(), new SecureRandom());
                ms.Flush();
                certificateData = ms.ToArray();
            }

            return new X509Certificate2(certificateData, tmpPassword, X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet);
        }

        public static X509Certificate2 CreateSelfSignedCert(
            SubjectAlternativeName subjectAlternativeName,
            DistinguishedName? distinguishedName,
            BasicConstraints basicConstraints,
            OidCollection enhancedKeyUsages,
            X509KeyUsageFlags x509KeyUsageFlags,
            CertificateLifetime? certificateLifetime,
            long? serialNumber,
            CertificateAlgorithmOptions options)
        {
            distinguishedName ??= new DistinguishedName(subjectAlternativeName);
            var lifetime = certificateLifetime ?? CertificateLifetime.AsFiveYears();

            if (!serialNumber.HasValue)
            {
                var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                var unixTime = Convert.ToInt64((DateTime.UtcNow - epoch).TotalSeconds);
                serialNumber = unixTime;
            }

            var serial = BigInteger.ValueOf(serialNumber.Value);
            var subject = new X509Name(distinguishedName.ToString());
            v3CertGen.Reset();
            v3CertGen.SetSubjectDN(subject);
            v3CertGen.SetIssuerDN(subject);
            v3CertGen.SetSerialNumber(serial);
            v3CertGen.AddExtension(X509Extensions.BasicConstraints, basicConstraints.Critical, basicConstraints.ToBouncyX509Extension());
            v3CertGen.AddExtension(X509Extensions.SubjectAlternativeName, true, subjectAlternativeName.ToBouncyX509Extension());
            v3CertGen.AddExtension(X509Extensions.KeyUsage, true, x509KeyUsageFlags.ToX509Extension());
            v3CertGen.AddExtension(X509Extensions.ExtendedKeyUsage, true, enhancedKeyUsages.ToX509Extension());
            v3CertGen.SetNotBefore(lifetime.NotBefore.Date);
            v3CertGen.SetNotAfter(lifetime.NotAfter.Date);
            var random = new SecureRandom();

            AsymmetricCipherKeyPair? kp = null;
            ISignatureFactory? signatureFactory = null;
            if (options is RsaCertificateOptions rsaOptions)
            {
                var rsaGenerator = new RsaKeyPairGenerator();
                var rsaParams = new KeyGenerationParameters(random, rsaOptions.KeySize);
                rsaGenerator.Init(rsaParams);

                kp = rsaGenerator.GenerateKeyPair();
                signatureFactory = new Asn1SignatureFactory("SHA256WITHRSA", kp.Private, random);
            }
            else if (options is EcdsaCertificateOptions ecdsaOptions)
            {
                var ecGenerator = new ECKeyPairGenerator("ECDSA");
                var ecParams = new KeyGenerationParameters(random, ecdsaOptions.KeySize);
                ecGenerator.Init(ecParams);

                kp = ecGenerator.GenerateKeyPair();
                signatureFactory = new Asn1SignatureFactory("SHA256WITHECSDA", kp.Private, random);
            }
            else
            {
                throw new NotSupportedException($"{options.GetType().FullName} is not supported");
            }

            v3CertGen.SetPublicKey(kp.Public);
            var cert = v3CertGen.Generate(signatureFactory);

            string alias = cert.SubjectDN.ToString();
            Pkcs12Store store = new Pkcs12StoreBuilder().Build();

            X509CertificateEntry certEntry = new X509CertificateEntry(cert);
            store.SetCertificateEntry(alias, certEntry);

            // TODO: This needs extra logic to support a certificate chain
            AsymmetricKeyEntry keyEntry = new AsymmetricKeyEntry(kp.Private);
            store.SetKeyEntry(alias, keyEntry, new X509CertificateEntry[] { certEntry });

            byte[] certificateData;
            var tmpPassword = "temp@#23d23421ds_+";
            using (var ms = new MemoryStream())
            {
                store.Save(ms, tmpPassword.ToCharArray(), new SecureRandom());
                ms.Flush();
                certificateData = ms.ToArray();
            }

            return new X509Certificate2(certificateData, tmpPassword, X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet);
        }

        public static Org.BouncyCastle.Asn1.X509.BasicConstraints ToBouncyX509Extension(this BasicConstraints constraints)
        {
            if (constraints.CertificateAuthority)
                return new Org.BouncyCastle.Asn1.X509.BasicConstraints(true);

            return new Org.BouncyCastle.Asn1.X509.BasicConstraints(constraints.PathLengthConstraint);
        }

        private static DerSequence ToBouncyX509Extension(this SubjectAlternativeName san)
        {
            return new DerSequence(
                    san.DnsNames.Select(name => new GeneralName(GeneralName.DnsName, name))
                                           .ToArray<Asn1Encodable>());
        }

        private static X509CertificateAlgorithm FromOid(string oid)
        {
            switch (oid)
            {
                case Oids.Rsa:
                    return X509CertificateAlgorithm.Rsa;

                case Oids.Ec:
                    return X509CertificateAlgorithm.Ec;

                case Oids.Dsa:
                    return X509CertificateAlgorithm.Dsa;

                default:
                    return X509CertificateAlgorithm.Unsupported;
            }
        }

        private static ExtendedKeyUsage ToX509Extension(this OidCollection collection)
        {
            var purposes = new List<DerObjectIdentifier>();
            foreach (var oid in collection)
            {
                switch (oid.Value)
                {
                    case Oids.ClientAuthentication:
                       purposes.Add(KeyPurposeID.IdKPClientAuth);
                       break;

                    case Oids.ServerAuthentication:
                       purposes.Add(KeyPurposeID.IdKPServerAuth);
                       break;

                    case Oids.CodeSigning:
                       purposes.Add(KeyPurposeID.IdKPCodeSigning);
                       break;

                    case Oids.TimeStamping:
                       purposes.Add(KeyPurposeID.IdKPTimeStamping);
                       break;

                    case Oids.SecureEmail:
                       purposes.Add(KeyPurposeID.IdKPEmailProtection);
                       break;

                    case Oids.IPsecurityTunnelTermination:
                       purposes.Add(KeyPurposeID.IdKPIpsecTunnel);
                       break;

                    case Oids.IPsecurityUser:
                       purposes.Add(KeyPurposeID.IdKPIpsecUser);
                       break;

                    case Oids.IPsecurityEndSystem:
                       purposes.Add(KeyPurposeID.IdKPIpsecEndSystem);
                       break;

                    case Oids.OCSPSigning:
                       purposes.Add(KeyPurposeID.IdKPOcspSigning);
                       break;

                    case Oids.SmartCardLogon:
                       purposes.Add(KeyPurposeID.IdKPSmartCardLogon);
                       break;

                    case Oids.MacAddress:
                       purposes.Add(KeyPurposeID.IdKPMacAddress);
                       break;

                    default:
                       purposes.Add(new DerObjectIdentifier(oid.Value));
                       break;
                }
            }

            return new ExtendedKeyUsage(purposes);
        }

        private static KeyUsage ToX509Extension(this X509KeyUsageFlags flags)
        {
            int usage = 0;

            if (flags.HasFlag(X509KeyUsageFlags.None))
                return new KeyUsage(0);

            if (flags.HasFlag(X509KeyUsageFlags.EncipherOnly))
                usage |= KeyUsage.EncipherOnly;

            if (flags.HasFlag(X509KeyUsageFlags.CrlSign))
                usage |= KeyUsage.CrlSign;

            if (flags.HasFlag(X509KeyUsageFlags.KeyCertSign))
                usage |= KeyUsage.KeyCertSign;

            if (flags.HasFlag(X509KeyUsageFlags.KeyAgreement))
                usage |= KeyUsage.KeyAgreement;

            if (flags.HasFlag(X509KeyUsageFlags.DataEncipherment))
                usage |= KeyUsage.DataEncipherment;

            if (flags.HasFlag(X509KeyUsageFlags.KeyEncipherment))
                usage |= KeyUsage.KeyEncipherment;

            if (flags.HasFlag(X509KeyUsageFlags.NonRepudiation))
                usage |= KeyUsage.NonRepudiation;

            if (flags.HasFlag(X509KeyUsageFlags.DigitalSignature))
                usage |= KeyUsage.KeyEncipherment;

            if (flags.HasFlag(X509KeyUsageFlags.DecipherOnly))
                usage |= KeyUsage.DecipherOnly;

            return new KeyUsage(usage);
        }
    }

    public class PasswordStore : IPasswordFinder
    {
        private readonly char[] password;

        public PasswordStore()
        {
            this.password = Array.Empty<char>();
        }

        public PasswordStore(ReadOnlySpan<char> password)
        {
            this.password = password.ToArray();
        }

        public PasswordStore(ReadOnlySpan<byte> password, Encoding? encoding)
        {
            encoding = encoding ?? new UTF8Encoding(false, true);

#if NETSTANDARD2_1_OR_GREATER
            Span<char> pw = stackalloc char[encoding.GetCharCount(password)];
            encoding.GetChars(password, pw);
            this.password = pw.ToArray();
#else
            var bytes = password.ToArray();
            this.password = encoding.GetChars(bytes);
            Array.Clear(bytes, 0, bytes.Length);
#endif
        }

        public void Clear()
        {
            Array.Clear(this.password, 0, this.password.Length);
        }

        public char[] GetPassword()
        {
            return this.password;
        }
    }
}
