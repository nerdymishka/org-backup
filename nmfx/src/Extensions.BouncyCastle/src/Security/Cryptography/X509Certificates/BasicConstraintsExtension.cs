using System;
using System.Collections.Generic;
using System.Text;
using Org.BouncyCastle.Asn1.X509;

namespace NerdyMishka.Security.Cryptography.X509Certificates
{
    public class BasicConstraintsExtension : IX509Extension
    {
        private readonly Org.BouncyCastle.Asn1.X509.BasicConstraints constraints;

        public BasicConstraintsExtension(BasicConstraints basicConstraints)
        {
            this.IsCritical = basicConstraints.Critical;
            if (basicConstraints.CertificateAuthority)
            {
                this.constraints = new Org.BouncyCastle.Asn1.X509.BasicConstraints(true);
                return;
            }

            this.constraints = new Org.BouncyCastle.Asn1.X509.BasicConstraints(basicConstraints.PathLengthConstraint);
        }

        public BasicConstraintsExtension(bool isCa, int pathLength, bool critical = false)
        {
            this.IsCritical = critical;
            if (isCa)
            {
                // path lenth is going to be zero;
                this.constraints = new Org.BouncyCastle.Asn1.X509.BasicConstraints(isCa);
                return;
            }

            this.constraints = new Org.BouncyCastle.Asn1.X509.BasicConstraints(pathLength);
        }

        public bool IsCritical { get; private set; }

        public string ObjectIdentifier => X509Extensions.BasicConstraints.Id;

        public byte[] Data => this.constraints.GetDerEncoded();
    }
}
