using System;
using System.Collections.Generic;
using System.Text;

namespace NerdyMiska.Security.Cryptography.X509Certificates
{
    public class BasicConstraints
    {
        /// <summary>
        /// Gets or sets a value indicating whether the certificate is a certificate authority.
        /// </summary>
        public bool CertificateAuthority { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the certificate has a path length constraint.
        /// </summary>
        public bool HasPathLengthConstraint { get; set; }

        /// <summary>
        /// Gets or sets the path length constraint.
        /// </summary>
        public int PathLengthConstraint { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the certificate is critical.
        /// </summary>
        public bool Critical { get; set; }
    }
}
