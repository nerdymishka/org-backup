using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Org.BouncyCastle.Asn1.X509;

namespace NerdyMishka.Security.Cryptography.X509Certificates
{
    public class KeyUsageExtension : IX509Extension
    {
        private readonly KeyUsage keyUsage;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "RCS1096:Convert 'HasFlag' call to bitwise operation (or vice versa).", Justification = "By Design")]
        public KeyUsageExtension(X509KeyUsageFlags flags)
        {
            int usage = 0;

            if (flags.HasFlag(X509KeyUsageFlags.None))
                this.keyUsage = new KeyUsage(0);

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

            this.keyUsage = new KeyUsage(usage);
        }

        public bool IsCritical => true;

        public string ObjectIdentifier => X509Extensions.KeyUsage.Id;

        public byte[] Data => this.keyUsage.GetDerEncoded();
    }
}
