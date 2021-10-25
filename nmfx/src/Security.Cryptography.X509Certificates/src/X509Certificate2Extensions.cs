using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace NerdyMishka.Security.Cryptography.X509Certificates
{
    public static class X509Certificate2Extensions
    {
        public static X509Certificate2 CopyWithoutPrivateKey(this X509Certificate2 certificate)
        {
            var publicKeyBytes = certificate.Export(X509ContentType.Cert);
            var signingCertWithoutPrivateKey = new X509Certificate2(publicKeyBytes);
            return signingCertWithoutPrivateKey;
        }

        public static X509Certificate2 ExportToStore(this X509Certificate2 certificate, X509Store store)
        {
            X509KeyStorageFlags flags = X509KeyStorageFlags.PersistKeySet;

            if (store.Location == StoreLocation.LocalMachine)
            {
                flags |= X509KeyStorageFlags.MachineKeySet;
            }
            else
            {
                // Required if your input key was already a persisted machine key
                flags |= X509KeyStorageFlags.UserKeySet;
            }

            var persistable = new X509Certificate2(certificate.Export(X509ContentType.Pfx), string.Empty, flags);
            store.Certificates.Add(persistable);

            return persistable;
        }

        /*
        public static ReadOnlySpan<char> ToPemEncodedPrivateKey(this X509Certificate2 certificate2)
        {
            var ecdsa = certificate2.GetECDsaPrivateKey();
            var sb = new System.Text.StringBuilder();
            if(ecdsa != null)
            {
                sb.AppendLine(PemFormat.GetBegin(PemTypes.EcPrivateKey));
                sb.AppendLine(Convert.ToBase64String(ecdsa.ExportECPrivateKey)
            }
        }*/

        public static byte[] HexToByteArray(this string value)
        {
            var bytes = new byte[value.Length / 2];

            for (var i = 0; i < value.Length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(value.Substring(i, 2), 16);
            }

            return bytes;
        }

        public static ReadOnlySpan<byte> ToPfxBytes(this X509Certificate2 certificate2, ReadOnlySpan<char> password, X509Certificate2? signingCertificate = null)
        {
            if (signingCertificate == null)
                return certificate2.ToPfxBytes(password, null, null);

            var caCertCollection = GetCertificateCollection(signingCertificate, password);
            var publicKeySigningCert = signingCertificate.CopyWithoutPrivateKey();

            return certificate2.ToPfxBytes(password, publicKeySigningCert, caCertCollection);
        }

        private static X509Certificate2Collection? GetCertificateCollection(
           X509Certificate2 inputCert,
           ReadOnlySpan<char> password)
        {
            var certificateCollection = new X509Certificate2Collection();
            certificateCollection.Import(
                inputCert.GetRawCertData(),
                password.ToString(),
                X509KeyStorageFlags.Exportable | X509KeyStorageFlags.UserKeySet);

            X509Certificate2? certificate = null;
            var outcollection = new X509Certificate2Collection();
            foreach (X509Certificate2 element in certificateCollection)
            {
                if (certificate == null && element.HasPrivateKey)
                {
                    certificate = element;
                }
                else
                {
                    outcollection.Add(element);
                }
            }

            return outcollection;
        }

        private static X509Certificate2Collection? GetCertificateCollection(
            X509Certificate2 inputCert,
            string password)
        {
            var certificateCollection = new X509Certificate2Collection();
            certificateCollection.Import(
                inputCert.GetRawCertData(),
                password,
                X509KeyStorageFlags.Exportable | X509KeyStorageFlags.UserKeySet);

            X509Certificate2? certificate = null;
            var outcollection = new X509Certificate2Collection();
            foreach (X509Certificate2 element in certificateCollection)
            {
                if (certificate == null && element.HasPrivateKey)
                {
                    certificate = element;
                }
                else
                {
                    outcollection.Add(element);
                }
            }

            return outcollection;
        }

        private static byte[]? ToPfxBytes(
            this X509Certificate2 certificate,
            string password,
            X509Certificate2? signingCertificate,
            X509Certificate2Collection? chain)
        {
            var certCollection = new X509Certificate2Collection(certificate);
            if (chain != null && chain.Count > 0)
            {
                certCollection.AddRange(chain);
            }

            if (signingCertificate != null)
            {
                var signingCertWithoutPrivateKey = signingCertificate.CopyWithoutPrivateKey();
                certCollection.Add(signingCertWithoutPrivateKey);
            }

            return certCollection.Export(X509ContentType.Pkcs12, password.ToString());
        }

        private static byte[]? ToPfxBytes(
            this X509Certificate2 certificate,
            ReadOnlySpan<char> password,
            X509Certificate2? signingCertificate,
            X509Certificate2Collection? chain)
        {
            var certCollection = new X509Certificate2Collection(certificate);
            if (chain != null && chain.Count > 0)
            {
                certCollection.AddRange(chain);
            }

            if (signingCertificate != null)
            {
                var signingCertWithoutPrivateKey = signingCertificate.CopyWithoutPrivateKey();
                certCollection.Add(signingCertWithoutPrivateKey);
            }

            return certCollection.Export(X509ContentType.Pkcs12, password.ToString());
        }
    }
}
